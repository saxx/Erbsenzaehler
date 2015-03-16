erbsenzaehlerServices.factory('linesEditorResource', function ($resource) {
    return $resource($('.lines-editor').data('action') + '?month=:month', { month: '' }, {
        query: { params: { month: '' } },
        update: { method: 'PUT', params: { month: '' } }
    });
});