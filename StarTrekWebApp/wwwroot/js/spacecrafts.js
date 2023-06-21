$(document).ready(() => {
    var editor = new DataTable.Editor({
        ajax: '/Spacecrafts?handler=TableData',
        type: 'POST',
        headers:
        {
            "RequestVerificationToken": $('input:hidden[name="__RequestVerificationToken"]').val()
        },
        table: '#dtSpacecrafts',
        idSrc: "uid",
        fields: [
            { label: 'Name', name: 'name' },
            { label: 'Registry', name: 'registry' },
            { label: 'Status', name: 'status' },
            { label: 'Date Status', name: 'dateStatus' }
        ]
    });

    $('#dtSpacecrafts').DataTable({
        dom: "Bfrtip",
        serverSide: true,
        ajax: {
            "url": "/Spacecrafts?handler=TableData",
            "type": "GET",
            "dataType": "json",
            "data": (data) => {
                data.search =
                {
                    "value": `${$('#dtSpacecrafts_filter input').val()}`
                }
            }
        },
        columns: [
            { data: 'name' },
            { data: 'registry' },
            { data: 'status' },
            { data: 'dateStatus' },
            {
                data: 'systemDate',
                render: (data) => formatDate(data)
            },
            {
                data: 'lastChange',
                render: (data) => formatDate(data)
            }
        ],
        select: 'single',
        buttons: [
            { extend: 'create', editor: editor },
            { extend: 'edit', editor: editor },
            { extend: 'remove', editor: editor }
        ]
    });

    function formatDate(dateStr) {
        if (dateStr == null)
            return "";
        var date = new Date(dateStr);
        var formattedDate = `${date.getFullYear()}-${(date.getMonth() + 1)
            .toString()
            .padStart(2, '0')}-${date.getDate().toString().padStart(2, '0')} ${date.getHours().toString().padStart(2, '0')}:${date.getMinutes().toString().padStart(2, '0')}:${date.getSeconds().toString().padStart(2, '0')}`;
        return formattedDate;
    }
});