erbsenzaehlerServices.factory('linesEditorResource', [
    '$resource',
    function ($resource) {
        return $resource($('#linesEditorController').data('action') + '?month=:month', { month: '' }, {
            query: { method: 'GET' },
            update: { method: 'PUT' },
            create: { method: 'POST' },
            delete: {
                method: 'DELETE',
                url: $('#linesEditorController').data('action') + '/:id',
                param: { month: '', id: '' }
            }
        });
    }
]);