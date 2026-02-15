$(function () {
    var detailsModalEl = document.getElementById('detailsModal');
    var detailsModal = detailsModalEl && window.bootstrap ? new bootstrap.Modal(detailsModalEl) : null;

    function showPopup(message) {
        alert(message);
    }

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
                    return msgs.join('\n');
                }

                return JSON.stringify(xhr.responseJSON);
            }

            if (xhr.responseText) return xhr.responseText;
            return defaultMsg || xhr.statusText || 'Request failed';
        } catch (e) {
            return defaultMsg || 'Request failed';
        }
    }

    function validateFormWithPopup(form) {
        var messages = [];
        var elements = form.querySelectorAll('input, textarea, select');

        elements.forEach(function (el) {
            if (!el.checkValidity()) {
                var label = $('label[for="' + el.id + '"]').first().text() || el.name || 'Field';
                var message = el.validationMessage || (label + ' is invalid.');
                messages.push(label + ': ' + message);
            }
        });

        if (messages.length > 0) {
            showPopup(messages.join('\n'));
            return false;
        }

        return true;
    }

    function copyHobbiesToHidden() {
        var sel = $('#HobbiesSelect').val();
        if (sel && Array.isArray(sel) && sel.length > 0) {
            $('#Hobbies').val(sel.join(','));
        } else {
            $('#Hobbies').val('');
        }
    }

    function resetFormMode() {
        $('#userForm')[0].reset();
        $('#Id').val('0');
        $('#Hobbies').val('');
        $('#saveBtn').show();
        $('#updateBtn').hide();
    }

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
                    '<button type="button" class="btn btn-sm btn-warning editBtn">Edit</button> ' +
                    '<button type="button" class="btn btn-sm btn-danger deleteBtn">Delete</button>' +
                    '</td></tr>';
            });

            $('#userGrid tbody').html(rows);
        }).fail(function (xhr) {
            showPopup('Error loading grid: ' + formatXhrError(xhr));
        });
    }

    function showDetails(id) {
        $.get('/User/Details', { id: id }, function (d) {
            var html =
                '<p><strong>Name:</strong> ' + (d.name || d.Name) + '</p>' +
                '<p><strong>Email:</strong> ' + (d.email || d.Email) + '</p>' +
                '<p><strong>Mobile:</strong> ' + (d.mobileNo || d.MobileNo) + '</p>' +
                '<p><strong>Address:</strong> ' + (d.address || d.Address) + '</p>' +
                '<p><strong>Gender:</strong> ' + (d.gender || d.Gender) + '</p>' +
                '<p><strong>State:</strong> ' + (d.state || d.State) + '</p>' +
                '<p><strong>Hobbies:</strong> ' + (d.hobbies || d.Hobbies) + '</p>';

            $('#detailsBody').html(html);
            if (detailsModal) {
                detailsModal.show();
            } else {
                showPopup($(html).text());
            }
        }).fail(function (xhr) {
            showPopup('Error loading details: ' + formatXhrError(xhr));
        });
    }

    loadGrid();

    $('#State').on('input focus', function () {
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
            showPopup('Error loading states: ' + formatXhrError(xhr));
        });
    });

    $('#userForm').submit(function (e) {
        e.preventDefault();

        var form = this;
        copyHobbiesToHidden();

        if (!validateFormWithPopup(form)) {
            return;
        }

        $.post('/User/Create', $(form).serialize())
            .done(function () {
                showPopup('Saved successfully');
                loadGrid();
                resetFormMode();
            })
            .fail(function (xhr) {
                showPopup('Save failed: ' + formatXhrError(xhr));
            });
    });

    $('#userGrid').on('click', '.editBtn', function (e) {
        e.stopPropagation();
        var id = $(this).closest('tr').data('id');

        $.get('/User/Details', { id: id }, function (d) {
            $('#Id').val(d.id || d.Id);
            $('#Name').val(d.name || d.Name);
            $('#Email').val(d.email || d.Email);
            $('#MobileNo').val(d.mobileNo || d.MobileNo);
            $('#Address').val(d.address || d.Address);
            $('#State').val(d.state || d.State);
            $('input[name="Gender"][value="' + (d.gender || d.Gender) + '"]').prop('checked', true);

            var hobbies = d.hobbies || d.Hobbies || '';
            var arr = Array.isArray(hobbies)
                ? hobbies
                : (typeof hobbies === 'string' && hobbies.length ? hobbies.split(',').map(function (s) { return s.trim(); }) : []);
            $('#HobbiesSelect').val(arr);
            $('#Hobbies').val(arr.join(','));

            $('#saveBtn').hide();
            $('#updateBtn').show();
        }).fail(function (xhr) {
            showPopup('Error loading user: ' + formatXhrError(xhr));
        });
    });

    $('#updateBtn').click(function (e) {
        e.preventDefault();

        var form = $('#userForm')[0];
        copyHobbiesToHidden();

        if (!validateFormWithPopup(form)) {
            return;
        }

        $.post('/User/Update', $('#userForm').serialize())
            .done(function () {
                showPopup('Updated successfully');
                loadGrid();
                resetFormMode();
            })
            .fail(function (xhr) {
                showPopup('Update failed: ' + formatXhrError(xhr));
            });
    });

    $('#resetBtn').click(function () {
        resetFormMode();
    });

    $('#userGrid').on('click', '.deleteBtn', function (e) {
        e.stopPropagation();
        if (!confirm('Confirm delete?')) return;

        var id = $(this).closest('tr').data('id');
        $.post('/User/Delete', { id: id })
            .done(function () {
                showPopup('Deleted successfully');
                loadGrid();
            })
            .fail(function (xhr) {
                showPopup('Delete failed: ' + formatXhrError(xhr));
            });
    });

    $('#userGrid').on('click', 'tbody tr', function () {
        var id = $(this).data('id');
        if (!id) return;

        showDetails(id);
    });
});
