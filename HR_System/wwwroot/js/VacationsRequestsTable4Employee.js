$(document).ready(function () {
    $('#vacationsRequestsTable').dataTable({
        "processing": true,
        "serverSide": true,
        "filter": true,
        "ajax": {
            "url": "/Employee/GetVacationsRequests",
            "type": "POST",
            "datatype": "json"
        },
        "columnDefs": [{
            "targets": [0],
            "visible": false,
            "searchable": false
        }],
        "columns": [
            { "data": "id", "name": "ID", "autoWidth": true, "orderable": false },
            { "data": "vacationType", "name": "Vacation Type", "autoWidth": true, "orderable": false },
            { "data": "startDate", "name": "Start Date", "autoWidth": true, "orderable": false },
            { "data": "endDate", "name": "End Date", "autoWidth": true, "orderable": false },
            { "data": "vacationDuration", "name": "Vacation Duration", "autoWidth": true, "orderable": false },
            { "data": "status", "name": "Status", "autoWidth": true, "orderable": false },
            {
                "render": function (data, type, row) {
                    return "<button onclick=\"location.href = '/Employee/RequestDetails/" + row.id + "'\" class='btn btn-primary'>Details</button>";
                }
                , "searchable": false, "orderable": false
            }
        ]
    });
});