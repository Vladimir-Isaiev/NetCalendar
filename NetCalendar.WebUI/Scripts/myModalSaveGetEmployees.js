$('#myModalSave').on('shown.bs.modal', function (e) {
    GetEmpl();
})


function GetEmpl() {
    var department = ($('#txtDepartment').val().trim());
    var url = "/Home/GetUsers/?department=" + department;

    var source =
        {
            datatype: "json",
            datafields: [
                { name: 'SimplyName' },
                { name: 'Email' }
            ],
            url: url,
            async: true
        };

    var dataAdapter = new $.jqx.dataAdapter(source);

    $("#jqxWidget").jqxDropDownList({ source: dataAdapter });
    
}
