erbsenzaehlerServices.factory('linesEditorResource', [
    '$resource',
    function ($resource) {
        return $resource($('.lines-editor').data('action') + '?month=:month', { month: '' }, {
            query: { params: { month: '' } },
            update: { method: 'POST', params: { month: '' } }
        });
    }
]);