(function () {
    const CH = new BroadcastChannel('img-sync-v1');
    const KEY = 'img-sync-v1-last';

    window.SyncBus = {
        send(msg) {
            try { CH.postMessage(msg); } catch { }
            try {
                localStorage.setItem(KEY, JSON.stringify({ t: Date.now(), msg }));
            } catch { }
        },
        on(handler) {
            CH.onmessage = (e) => handler(e.data);
            window.addEventListener('storage', (e) => {
                if (e.key === KEY && e.newValue) {
                    try { const { msg } = JSON.parse(e.newValue); handler(msg); } catch { }
                }
            });
        }
    };
})();
