erbsenzaehlerServices.factory('linesEditorResource', function ($resource) {
    return $resource($('.lines-editor').data('action') + '?month=:month', { month: '' }, {
        query: { method: 'GET' },
        update: { method: 'PUT' },
        delete: {
            method: 'DELETE', 
            url: $('.lines-editor').data('action') + '/:id/?month=:month',
            param: { month: '', id: '' }
    }
    });
});