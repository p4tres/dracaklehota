const ImageViewer = (function () {
    let viewport, img, overlay, ctx, role = 'master';
    let scale = 1, tx = 0, ty = 0;
    let isPanning = false, lastX = 0, lastY = 0;

    let mode = 'cursor';
    let brush = 30;
    let isDrawing = false;
    let lastImgX = 0, lastImgY = 0;

    let applyingRemote = false;

    const KEY_IMAGE = 'img-sync-v1-current-image';
    const KEY_STATE = 'img-sync-v1-current-state';
    const KEY_DRAW = 'img-sync-v1-draw-log';

    function ensureOverlay() {
        if (overlay) return;
        overlay = document.createElement('canvas');
        overlay.className = 'image';
        overlay.style.position = 'absolute';
        overlay.style.left = '0px';
        overlay.style.top = '0px';
        overlay.style.pointerEvents = 'none';
        viewport.appendChild(overlay);
        ctx = overlay.getContext('2d');
    }

    function mount(opts) {
        viewport = document.getElementById(opts.elementIds.viewport);
        img = document.getElementById(opts.elementIds.image);
        role = opts.role || 'master';

        img.draggable = false;
        ensureOverlay();

        viewport.addEventListener('wheel', onWheel, { passive: false });
        viewport.addEventListener('pointerdown', onDown);
        viewport.addEventListener('pointermove', onMove);
        window.addEventListener('pointerup', onUp);
        window.addEventListener('keydown', onKey);

        SyncBus.on((msg) => {
            if (!msg) return;
            if (msg.type === 'state') {
                applyingRemote = true;
                setState(msg.payload.scale, msg.payload.tx, msg.payload.ty, /*broadcast*/ false);
                applyingRemote = false;
            } else if (msg.type === 'image' && msg.payload?.src) {
                // Na nový obrázok zrušíme overlay a vyčistíme lokálny zápis (u followera treba clear)
                load(msg.payload.src, /*broadcast*/ false);
                // follower si neukladá draw log; príde zo storage pri rehydratácii (ak je master v tom istom origin)
            } else if (msg.type === 'draw' && role === 'follower') {
                applyRemoteDraw(msg.payload);
            }
        });

        updateCursor();
    }

    // Promise-based load (resolve po onload)
    function load(src, broadcast = true) {
        return new Promise(resolve => {
            img.onload = () => {
                ensureOverlay();
                overlay.width = img.naturalWidth;
                overlay.height = img.naturalHeight;
                ctx.clearRect(0, 0, overlay.width, overlay.height);
                // Na nový obrázok nulujeme draw log (iba na masteri, follower sa riadi tým, čo je v storage)
                if (role === 'master') {
                    try { localStorage.setItem(KEY_DRAW, JSON.stringify([])); } catch { }
                }
                setState(1, 0, 0, broadcast);
                resolve();
            };
            img.src = src;
            try { localStorage.setItem(KEY_IMAGE, src); } catch { }
            if (broadcast) {
                SyncBus.send({ type: 'image', payload: { src } });
            }
        });
    }

    function onWheel(e) {
        e.preventDefault();
        const rect = viewport.getBoundingClientRect();
        const cx = e.clientX - rect.left;
        const cy = e.clientY - rect.top;

        const delta = e.deltaY;
        const factor = Math.pow(1.0015, -delta);
        const newScale = clamp(scale * factor, 0.05, 50);

        const dx = cx - tx;
        const dy = cy - ty;
        const nx = cx - dx * (newScale / scale);
        const ny = cy - dy * (newScale / scale);

        setState(newScale, nx, ny, true);
    }

    function toImageSpace(clientX, clientY) {
        const rect = viewport.getBoundingClientRect();
        const sx = clientX - rect.left;
        const sy = clientY - rect.top;
        const ix = (sx - tx) / scale;
        const iy = (sy - ty) / scale;
        return { x: ix, y: iy };
    }

    function onDown(e) {
        if (role === 'follower') return;

        if (mode === 'cursor') {
            isPanning = true; lastX = e.clientX; lastY = e.clientY; viewport.setPointerCapture(e.pointerId);
            return;
        }

        if (mode === 'add' || mode === 'erase') {
            isDrawing = true;
            viewport.setPointerCapture(e.pointerId);
            const p = toImageSpace(e.clientX, e.clientY);
            lastImgX = p.x; lastImgY = p.y;
            const seg = { x1: p.x, y1: p.y, x2: p.x, y2: p.y, size: brush, mode };
            drawSegment(seg);
            logDraw(seg);   // uložiť len na masteri
            sendDraw(seg);  // broadcast
        }
    }

    function onMove(e) {
        if (role === 'follower') return;

        if (mode === 'cursor') {
            if (!isPanning) return;
            const dx = e.clientX - lastX; const dy = e.clientY - lastY;
            lastX = e.clientX; lastY = e.clientY;
            setState(scale, tx + dx, ty + dy, true);
            return;
        }

        if ((mode === 'add' || mode === 'erase') && isDrawing) {
            const p = toImageSpace(e.clientX, e.clientY);
            const seg = { x1: lastImgX, y1: lastImgY, x2: p.x, y2: p.y, size: brush, mode };
            drawSegment(seg);
            logDraw(seg);
            sendDraw(seg);
            lastImgX = p.x; lastImgY = p.y;
        }
    }

    function onUp() {
        isPanning = false;
        isDrawing = false;
    }

    function onKey(e) {
        if (role === 'follower') return;
        const step = 40;
        if (e.key === '+') { setState(clamp(scale * 1.1, 0.05, 50), tx, ty, true); }
        else if (e.key === '-') { setState(clamp(scale / 1.1, 0.05, 50), tx, ty, true); }
        else if (e.key === '0') { setState(1, 0, 0, true); }
        else if (e.key === 'ArrowLeft') { setState(scale, tx + step, ty, true); }
        else if (e.key === 'ArrowRight') { setState(scale, tx - step, ty, true); }
        else if (e.key === 'ArrowUp') { setState(scale, tx, ty + step, true); }
        else if (e.key === 'ArrowDown') { setState(scale, tx, ty - step, true); }
    }

    function setState(s, x, y, broadcast) {
        scale = s; tx = x; ty = y;
        const t = `translate(${tx}px, ${ty}px) scale(${scale})`;
        img.style.transform = t;
        if (overlay) overlay.style.transform = t;

        try { localStorage.setItem(KEY_STATE, JSON.stringify({ scale, tx, ty })); } catch { }

        if (broadcast && !applyingRemote) {
            SyncBus.send({ type: 'state', payload: { scale, tx, ty } });
        }
    }

    function drawSegment(seg) {
        if (!ctx) return;
        ctx.save();
        if (seg.mode === 'add') {
            ctx.globalCompositeOperation = 'source-over';
            ctx.strokeStyle = '#000';
        } else { // erase
            ctx.globalCompositeOperation = 'destination-out';
            ctx.strokeStyle = 'rgba(0,0,0,1)';
        }
        ctx.lineWidth = seg.size;
        ctx.lineCap = 'round';
        ctx.lineJoin = 'round';
        ctx.beginPath();
        ctx.moveTo(seg.x1, seg.y1);
        ctx.lineTo(seg.x2, seg.y2);
        ctx.stroke();
        ctx.restore();
    }

    function sendDraw(seg) {
        SyncBus.send({ type: 'draw', payload: seg });
    }

    function applyRemoteDraw(seg) {
        ensureOverlay();
        drawSegment(seg);
    }

    // Uloženie segmentu do draw-logu (iba master)
    function logDraw(seg) {
        if (role !== 'master') return;
        try {
            const raw = localStorage.getItem(KEY_DRAW);
            const arr = raw ? JSON.parse(raw) : [];
            arr.push(seg);
            // TOTO - veľkosť localstorage je limited
            localStorage.setItem(KEY_DRAW, JSON.stringify(arr));
        } catch { }
    }

    function setMode(newMode) {
        mode = newMode;
        updateCursor();
    }

    function setBrush(sz) {
        brush = Math.max(10, Math.min(100, sz | 0));
    }

    function updateCursor() {
        if (!viewport) return;
        viewport.classList.remove('cursor-move', 'cursor-draw');
        if (mode === 'cursor') viewport.classList.add('cursor-move');
        else viewport.classList.add('cursor-draw');
    }

    function clamp(v, lo, hi) { return Math.max(lo, Math.min(hi, v)); }

    // Rehydratácia: obrázok + stav + HISTÓRIA KRESLENIA
    async function rehydrateFromStorage() {
        try {
            const src = localStorage.getItem(KEY_IMAGE);
            const rawSt = localStorage.getItem(KEY_STATE);
            const rawDw = localStorage.getItem(KEY_DRAW);

            if (src) {
                await load(src, /*broadcast*/ false);
            }
            if (rawDw) {
                const arr = JSON.parse(rawDw);
                if (Array.isArray(arr)) {
                    // nakresli všetky uložené segmenty (v poradí)
                    arr.forEach(seg => applyRemoteDraw(seg));
                }
            }
            if (rawSt) {
                const st = JSON.parse(rawSt);
                if (st && typeof st.scale === 'number' && typeof st.tx === 'number' && typeof st.ty === 'number') {
                    setState(st.scale, st.tx, st.ty, /*broadcast*/ false);
                }
            }
        } catch { }
    }

    return { mount, load, setMode, setBrush, rehydrateFromStorage };
})();
