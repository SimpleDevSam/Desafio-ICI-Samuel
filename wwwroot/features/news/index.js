
(function () {
  const host = document.getElementById('newsFormHost');

    function renderErrors(form, payload) {
    const summary = form.querySelector('[data-validation-summary]');
    if (summary) {
        let messages = [];
    if (payload && payload.errors) {
        messages = Object.values(payload.errors).flat();
      } else if (payload && payload.message) {
        messages = [payload.message];
      }
    if (messages.length === 0) {
        messages = ['Failed to save.'];
      }
      summary.innerHTML = messages.map(m => `<div>${m}</div>`).join('');
    }
  }

    function wireForm(form) {
        form.addEventListener('submit', async function (e) {
            e.preventDefault();

            const action = form.getAttribute('data-action') || form.action;
            const body = new URLSearchParams(new FormData(form)); // includes antiforgery
            const resp = await fetch(action, {
                method: 'POST',
                headers: { 'Accept': 'application/json' },
                body
            });

            if (resp.ok) {
                const json = await resp.json();
                if (json && json.success) {
                    window.location = json.redirectUrl;
                } else {
                    renderErrors(form, json);
                }
            } else {
                try {
                    const errors = await resp.json();
                    renderErrors(form, errors);
                } catch {
                    alert('Failed to save.');
                }
            }
        });

    const cancel = form.querySelector('[data-close-news-form]');
    if (cancel) {
        cancel.addEventListener('click', () => {
            host.replaceChildren();
        });
    }
  }

    const createBtn = document.getElementById('btnCreateNews');
    if (createBtn) {
        createBtn.addEventListener('click', async () => {
            const resp = await fetch('/news/create', { headers: { 'X-Requested-With': 'XMLHttpRequest' } });
            const html = await resp.text();
            host.innerHTML = html;
            const form = host.querySelector('#newsForm');
            if (form) {
                wireForm(form);
            }
        });
    }

    document.querySelectorAll('[data-view-news]').forEach(btn => {
        btn.addEventListener('click', async () => {
            const id = btn.getAttribute('data-view-news');
            const resp = await fetch(`/news/${id}/view`, { headers: { 'X-Requested-With': 'XMLHttpRequest' } });
            const html = await resp.text();
            host.innerHTML = html;
            const viewElement = host.querySelector('.card');
            if (viewElement) {
                wireView(viewElement);
            }
        });
    });

  document.querySelectorAll('[data-edit-news]').forEach(btn => {
        btn.addEventListener('click', async () => {
            const id = btn.getAttribute('data-edit-news');
            const resp = await fetch(`/news/${id}/edit`, { headers: { 'X-Requested-With': 'XMLHttpRequest' } });
            const html = await resp.text();
            host.innerHTML = html;
            const form = host.querySelector('#newsForm');
            if (form) {
                wireForm(form);
            }
        });
  });
})();
