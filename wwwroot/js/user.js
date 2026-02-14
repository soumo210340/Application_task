$(function () {

    // ---------------- ALERT ----------------
    function showAlert(message) {
        alert(message);
    }

    // ---------------- FORMAT ERROR ----------------
    function formatXhrError(xhr, defaultMsg) {
        try {
            if (!xhr) return defaultMsg || 'Unknown error';

            if (xhr.responseJSON) {
                if (typeof xhr.responseJSON === 'string') return xhr.responseJSON;
                if (xhr.responseJSON.error) return xhr.responseJSON.error;

                if (xhr.responseJSON.errors) {
                    var msgs = [];
                    Object.keys(xhr.responseJSON.errors).forEach(function (k) {
                        msgs = msgs.concat(xhr.responseJSON.errors[k]);
                    });
                    return msgs.join('; ');
                }

                return JSON.stringify(xhr.responseJSON);
            }

            if (xhr.responseText) return xhr.responseText;
            return defaultMsg || xhr.statusText || 'Request failed';

        } catch (e) {
            return defaultMsg || 'Request failed';
        }
    }

    // ---------------- LOAD GRID ----------------
    function loadGrid() {
        $.get('/User/List', function (data) {

            var rows = '';

            data.forEach(function (d) {
                rows += '<tr data-id="' + (d.id || d.Id) + '">' +
                    '<td>' + (d.name || d.Name) + '</td>' +
                    '<td>' + (d.email || d.Email) + '</td>' +
                    '<td>' + (d.mobileNo || d.MobileNo) + '</td>' +
                    '<td>' + (d.state || d.State) + '</td>' +
                    '<td>' + (d.hobbies || d.Hobbies) + '</td>' +
                    '<td>' +
                    '<button type="button" class="btn btn-sm btn-info detailsBtn">Details</button> ' +
                    '<button type="button" class="btn btn-sm btn-warning editBtn">Edit</button> ' +
                    '<button type="button" class="btn btn-sm btn-danger deleteBtn">Delete</button>' +
                    '</td></tr>';
            });

            $('#userGrid tbody').html(rows);

        }).fail(function (xhr) {
            showAlert('Error loading grid: ' + formatXhrError(xhr));
        });
    }

    loadGrid();

    // ---------------- STATE AUTOCOMPLETE ----------------
    $('#State').on('input', function () {

        var term = $(this).val();

        $.get('/User/GetStates', { term: term }, function (data) {

            var options = '';

            data.forEach(function (s) {
                options += '<option value="' + (s.text || s.Text) + '">';
            });

            if ($('#statesList').length === 0) {
                $('#State').after('<datalist id="statesList"></datalist>');
            }

            $('#statesList').html(options);
            $('#State').attr('list', 'statesList');

        }).fail(function (xhr) {
            showAlert('Error loading states: ' + formatXhrError(xhr));
        });
    });

    // copy hobbies from HobbiesSelect into hidden Hobbies input
    function copyHobbiesToHidden() {
        try {
            var sel = $('#HobbiesSelect').val();
            if (sel && Array.isArray(sel)) {
                $('#Hobbies').val(sel.join(','));
            } else {
                $('#Hobbies').val('');
            }
        } catch (e) {
            // ignore
        }
    }

    // ---------------- SAVE ----------------
    $('#userForm').submit(function (e) {

        e.preventDefault();

        var form = this;

        if (!form.checkValidity()) {
            showAlert('Please fill all required fields correctly.');
            return;
        }

        copyHobbiesToHidden();

        var data = $(form).serialize();

        $.post('/User/Create', data)
            .done(function () {
                showAlert('Saved successfully');
                loadGrid();
                form.reset();
            })
            .fail(function (xhr) {
                showAlert('Save failed: ' + formatXhrError(xhr));
            });
    });

    // ---------------- EDIT ----------------
    $('#userGrid').on('click', '.editBtn', function () {

        var id = $(this).closest('tr').data('id');

        $.get('/User/Details', { id: id }, function (d) {

            $('#Id').val(d.id || d.Id);
            $('#Name').val(d.name || d.Name);
            $('#Email').val(d.email || d.Email);
            $('#MobileNo').val(d.mobileNo || d.MobileNo);
            $('#Address').val(d.address || d.Address);
            $('#State').val(d.state || d.State);

            $('input[name="Gender"][value="' + (d.gender || d.Gender) + '"]').prop('checked', true);

            try {
                var hobbies = d.hobbies || d.Hobbies || '';
                var arr = [];
                if (Array.isArray(hobbies)) arr = hobbies;
                else if (typeof hobbies === 'string' && hobbies.length) arr = hobbies.split(',').map(function (s) { return s.trim(); });

                // set multiselect and hidden
                $('#HobbiesSelect').val(arr);
                $('#Hobbies').val(arr.join(','));
            } catch (err) { }

            $('#saveBtn').hide();
            $('#updateBtn').show();

        }).fail(function (xhr) {
            showAlert('Error loading user: ' + formatXhrError(xhr));
        });
    });

    // ---------------- UPDATE ----------------
    $('#updateBtn').click(function (e) {

        e.preventDefault();

        var form = $('#userForm')[0];

        if (!form.checkValidity()) {
            showAlert('Please fill all required fields correctly.');
            return;
        }

        copyHobbiesToHidden();

        var data = $("#userForm").serialize();

        $.post('/User/Update', data)
            .done(function () {
                showAlert('Updated successfully');
                loadGrid();
                form.reset();
                $('#saveBtn').show();
                $('#updateBtn').hide();
            })
            .fail(function (xhr) {
                showAlert('Update failed: ' + formatXhrError(xhr));
            });
    });

    // ---------------- RESET ----------------
    $('#resetBtn').click(function () {
        $('#userForm')[0].reset();
        $('#saveBtn').show();
        $('#updateBtn').hide();
    });

    // ---------------- DELETE ----------------
    $('#userGrid').on('click', '.deleteBtn', function () {

        if (!confirm('Confirm delete?')) return;

        var id = $(this).closest('tr').data('id');

        $.post('/User/Delete', { id: id })
            .done(function () {
                showAlert('Deleted successfully');
                loadGrid();
            })
            .fail(function (xhr) {
                showAlert('Delete failed: ' + formatXhrError(xhr));
            });
    });

    // ---------------- DETAILS ----------------
    $('#userGrid').on('click', '.detailsBtn', function () {

        var id = $(this).closest('tr').data('id');

        $.get('/User/Details', { id: id }, function (d) {

            var html =
                '<p><strong>Name:</strong> ' + (d.name || d.Name) + '</p>' +
                '<p><strong>Email:</strong> ' + (d.email || d.Email) + '</p>' +
                '<p><strong>Mobile:</strong> ' + (d.mobileNo || d.MobileNo) + '</p>' +
                '<p><strong>Address:</strong> ' + (d.address || d.Address) + '</p>' +
               
