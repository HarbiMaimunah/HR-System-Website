$(document).ready(function () {
    $('#vacationsRequestsTable').dataTable({
        "processing": true,
        "serverSide": true,
        "filter": true,
        "ajax": {
            "url": "/Manager/GetVacationsRequests",
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
            { "data": "employeeID", "name": "Employee ID", "autoWidth": true, "orderable": false },
            {
                "render": function (data, type, row) {
                    return "<button onclick=\"location.href = '/Manager/VacationRequestResponse/" + row.id + "'\" class='btn btn-primary'>Respond</button>"
                        + " <button onclick=\"location.href = '/Manager/VacationRequestDetails/" + row.id + "'\" class='btn btn-secondary'>Details</button> ";
                }
                , "searchable": false, "orderable": false
            }
        ]
    });
}); 