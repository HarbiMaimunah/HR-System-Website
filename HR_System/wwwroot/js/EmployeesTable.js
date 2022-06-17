$(document).ready(function () {
    $('#employeesTable').dataTable({
        "processing": true,
        "serverSide": true,
        "filter": true,
        "ajax": {
            "url": "/Manager/GetEmployees",
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
            { "data": "firstName", "name": "First Name", "autoWidth": true, "orderable": false },
            { "data": "lastName", "name": "Last Name", "autoWidth": true, "orderable": false },
            { "data": "mobileNumber", "name": "Mobile Number", "autoWidth": true, "orderable": false },
            { "data": "emailAddress", "name": "Email Address", "autoWidth": true, "orderable": false },
            { "data": "jobTitle", "name": "Job Title", "autoWidth": true, "orderable": false },
            {
                "render": function (data, type, row) {
                    return "<button type='button' onclick=\"location.href = '/Manager/EditEmployee/" + row.id + "'\" class='btn btn-primary'>Edit</button>"
                        + " <button type='button' onclick=\"location.href = '/Manager/DeleteEmployee/" + row.id + "'\" class='btn btn-danger'>Delete</button> ";
                }
                , "searchable": false, "orderable": false
            }
        ]
    });
});