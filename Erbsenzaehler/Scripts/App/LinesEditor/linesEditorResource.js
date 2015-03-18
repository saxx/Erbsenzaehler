erbsenzaehlerServices.factory('linesEditorResource', function ($resource) {
    return $resource($('#linesEditorController').data('action') + '?month=:month', { month: '' }, {
        query: { method: 'GET' },
        update: { method: 'PUT' },
        create: { method: 'POST' },
        delete: {
            method: 'DELETE', 
            url: $('.lines-editor').data('action') + '/:id/?month=:month',
            param: { month: '', id: '' }
    }
    });
});