$(document).ready(function () {
    var events = [];
    var selectedEvent = null;
    var formatDate;
    var defaultFormat = "DD/MM/YYYY H:mm";

    var culture = $('#language').val().substr(3, 2);
    
    if (culture == 'US') {
        formatDate = "MM.DD.YYYY h:mm A";
    }
    else {
        formatDate = "DD/MM/YYYY H:mm";
    }
    var t = new Date();
    
    var currentTimeZoneOffset = t.getTimezoneOffset() / 60 * (-1);
   
    $("#jqxWidget").jqxDropDownList({
        template: "warning",
        selectedIndex: 0,
        filterable: true,
        displayMember: "SimplyName",
        valueMember: "Email",
        width: 200,
        height: 25,
        checkboxes: true
    });


    $("#jqxWidget").on('select', function (event) {
        if (event.args) {
            var items = $("#jqxWidget").jqxDropDownList('getCheckedItems');
            var checkedItems = "";
            $.each(items, function (index) {
                checkedItems += this.label + ", ";
            });
            $("#txtDescription").val(checkedItems);
        }
    });
   
    FetchEventAndRenderCalendar();
    function FetchEventAndRenderCalendar() {
        events = [];
        $.ajax({
            type: "GET",
            url: "/home/GetEvents",
            success: function (data) {
                $.each(data, function (i, v) {

                    var s = moment(v.Start).toDate();
                    var e = moment(v.End).toDate();

                    s.setHours(s.getHours() + currentTimeZoneOffset);
                    e.setHours(e.getHours() + currentTimeZoneOffset);

                    events.push({
                        Id: v.Id,
                        googleEventId: v.GoogleEventId,
                        department: v.Department,
                        title: v.Subject,
                        subject: v.Subject,
                        description: v.EmployeesString,
                        start: s,
                        end: e,
                        color: v.ThemeColor,
                        destinationLat: v.DestLat,
                        destinationLong: v.DestLong,
                        adress: v.Adress,
                        allDay: false
                    });
                })

                GenerateCalender(events);
            },
            error: function (error) {
                alert('Render Calendar failed');
            }
        })
    }

    function GenerateCalender(events) {
        $('#calender').fullCalendar('destroy');
        $('#calender').fullCalendar({
            contentHeight: 400,
            defaultDate: new Date(),
            timeFormat: 'hh:mm a',
            header: {
                left: 'prev,next today',
                center: 'title',
                right: 'month,basicWeek,basicDay,agenda'
            },
            eventLimit: true,
            eventColor: 'Green',
            events: events,
            eventClick: function (calEvent, jsEvent, view) {
                selectedEvent = calEvent;
                
                $('#myModal #eventSubject').text(calEvent.subject);

                var $description = $('<div/>');
                $description.append($('<p/>').html('<b>Start:</b>' + calEvent.start.format(formatDate)));

                if (calEvent.end != null) {
                    $description.append($('<p/>').html('<b>End:</b>' + calEvent.end.format(formatDate)));
                }

                $description.append($('<p/>').html('<b>Department:</b>' + calEvent.department));
                $description.append($('<p/>').html('<b>Adress:</b>' + calEvent.adress));

                if (calEvent.destinationLat != null || calEvent.destinationLong != null) {
                    $description.append($('<p/>').html('<b>Des Lat:</b>' + calEvent.destinationLat));
                    $description.append($('<p/>').html('<b>Des Long:</b>' + calEvent.destinationLong));
                }

                $('#myModal #pDetails').empty().html($description);

                $('#myModal').modal();
            },
            selectable: true,
            select: function (start, end) {
                if (isPeriod(start, end)) {
                    sumEvent(start, end);
                }
                else {
                    var isManager = $('#isManager').val();
                    if (isManager == "False") {
                        alert("Only the manager can create event");
                        return;
                    }
                    selectedEvent = {
                        Id: 0,
                        googleEventId: "",
                        subject: "",
                        department: '',
                        description: '',
                        start: start,
                        end: end,
                        destinationLat: 0,
                        destinationLong: 0,
                        color: '',
                        adress: '',
                        allDay: false
                    };
                    openAddEditForm();
                    $('#calendar').fullCalendar('unselect');
                }
            },
            editable: true,
            eventDrop: function (event) {
               
                var data = {
                    Id: event.Id,
                    GoogleEventId: event.googleEventId,
                    Department: event.department,
                    Subject: event.subject,
                    Start: event.start,
                    End: event.end,
                    Description: event.description,
                    ThemeColor: event.color,
                    DestLat: event.destinationLat,
                    DestLong: event.destinationLong,
                    Adress: event.adress,
                    Offset: currentTimeZoneOffset,
                    allDay: false
                };
                SaveEvent(data);
            }
        })
    }


    function isPeriod(start, end) {
        var check = true;
        var dateStart = new Date(start);
        var dateEnd = new Date(end);

        var startPlus = moment(start).add(1, "day");
        var startDatePlus = new Date(startPlus);

        if (
            (dateEnd.getMonth() == dateStart.getMonth() && ((dateEnd.getDate() - dateStart.getDate()) == 1)) ||
            (dateEnd.getMonth() != dateStart.getMonth() && startDatePlus.getDate() == dateEnd.getDate())
        ) {
            check = false;
        }
        return check;
    }


    function sumEvent(start, end) {
        var s = String(start.format(defaultFormat));
        var e = String(end.format(defaultFormat));
        $.ajax({
            type: "POST",
            url: '/home/SumEvent',
            data: { 'start': s, 'end': e },
            success: function (data) {                
                    alert(data.summa);
            },
            error: function () {
                alert('sum event Failed');
            }
        })
    }


    $('#btnEdit').click(function () {
        //Open modal dialog for edit event
        openAddEditForm();
    })
    $('#btnDelete').click(function () {
        var isManager = $('#isManager').val();
        if (isManager == "False") {
            alert("Only the manager can delete event");
            return;
        }
        if (selectedEvent != null && confirm('Are you sure?')) {
            $.ajax({
                type: "POST",
                url: '/home/DeleteEvent',
                data: { 'googleId': selectedEvent.googleEventId, 'department': selectedEvent.department },
                success: function (data) {
                    if (data.status) {
                        FetchEventAndRenderCalendar();
                        $('#myModal').modal('hide');
                    }
                },
                error: function () {
                    alert('btnDelete Failed');
                    FetchEventAndRenderCalendar();
                    $('#myModal').modal('hide');
                }
            })
        }
        selectedEvent = null;
    })

    $('#dtp1,#dtp2').datetimepicker({ format: formatDate });

    function openAddEditForm() {
        $('#txtStart').val("");
        $('#txtEnd').val("");
        
        if (selectedEvent != null) {
            $('#hdId').val(selectedEvent.Id);
            $('#hdGoogleEventId').val(selectedEvent.googleEventId);
            $('#txtSubject').val(selectedEvent.subject);
            $('#txtStart').val(selectedEvent.start.format(formatDate));
            $('#txtEnd').val(selectedEvent.end.format(formatDate));
            $('#txtDescription').val(selectedEvent.description);
            $('#hdDestinationLat').val(selectedEvent.destinationLat);
            $('#hdDestinationLong').val(selectedEvent.destinationLong);
            $('#ddThemeColor').val(selectedEvent.color);
            $('#adr').val(selectedEvent.adress);
        }
        $('#myModal').modal('hide');
        $('#myModalSave').modal();
    }

    $('#btnDest').click(function () {
        $('#myModalSave').modal('hide');
        $('#myModalMap').modal();
    })

    $('#btnDestSave').click(function () {
        $('#myModalMap').modal('hide');
        $('#myModalSave').modal();
    })

    $('#btnSave').click(function () {
        var isManager = $('#isManager').val();
        if (isManager == "False") {
            alert("Only the manager can create event");
            return;
        }
        if ($('#txtSubject').val().trim() == "") {
            alert('Subject required');
            return;
        }

        if ($('#txtDepartment').val().trim() == "") {
            alert('Department required');
            return;
        }

        if ($('#txtStart').val().trim() == "") {
            alert('Start date required');
            return;
        }

        if ($('#txtEnd').val().trim() == "") {
            alert('End date required');
            return;
        }

        if ($('#adr').val().trim() == "") {
            alert('Destination required');
            return;
        }


        var startDate = moment($('#txtStart').val(), formatDate).toDate();
        var endDate = moment($('#txtEnd').val(), formatDate).toDate();

        if (startDate > endDate) {
            alert('Invalid end date');
            return;
        }

        var s = startDate.getHours();
        var e = endDate.getHours();

        var rs = s - currentTimeZoneOffset;
        var er = e - currentTimeZoneOffset;

       
        startDate.setHours(rs);
        endDate.setHours(er);

        alert(moment(startDate).toISOString());

        var data = {
            Id: $('#hdId').val(),
            GoogleEventId: $('#hdGoogleEventId').val().trim(),
            Department: $('#txtDepartment').val().trim(),
            Subject: $('#txtSubject').val().trim(),
            Start: moment(startDate).toISOString(),
            End: moment(endDate).toISOString(),
            Description: $('#txtDescription').val(),
            ThemeColor: $('#ddThemeColor').val(),
            DestLat: $('#hdDestinationLat').val(),
            DestLong: $('#hdDestinationLong').val(),
            Offset: 0,
            Adress: $("#adr").val(),
            allDay: false
        }

        
        SaveEvent(data);
        selectedEvent = null;
    })

    function SaveEvent(data) {
        $.ajax({
            type: "POST",
            url: '/home/SaveEventAsync',
            data: data,
            success: function (data) {
                alert(data.mess);                    
                FetchEventAndRenderCalendar();
                $('#myModalSave').modal('hide');
            },
            error: function () {
                alert('SaveEvent Failed');
                FetchEventAndRenderCalendar();
                $('#myModalSave').modal('hide');
            }
        })
    }
})