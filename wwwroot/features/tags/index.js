(function () {
    const token = document.querySelector('#__af input[name="__RequestVerificationToken"]').value;

    document.querySelectorAll('[data-delete-tag]').forEach(btn => {
        btn.addEventListener('click', async (e) => {
            const id = btn.getAttribute('data-delete-tag');

            if (!confirm('Deletar essa tag?')) {
                return;
            }

            try {
                const resp = await fetch(`/tags/${id}/delete`, {
                    method: 'DELETE',
                    headers: { 'RequestVerificationToken': token, 'Accept': 'text/plain' }
                });

                if (resp.status === 204) {
                    document.getElementById(`row-${id}`)?.remove();
                } else {
                    const text = await resp.text();
                    alert(text || 'Failed to delete tag.');
                }
            } catch (err) {
                alert('Network error while deleting tag.');
            }
        });
    });
})();