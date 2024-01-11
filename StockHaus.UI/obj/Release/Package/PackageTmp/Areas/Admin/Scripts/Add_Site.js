$(document).ready(function () {


});

function dfnWS() {
    var id = 1903;

    $.ajax({
        type: "GET",
        url: "/Admin/Add/DefineStock/" + id,
        //contentType: "application/html charset=utf-8",
        dataType: "html",
        cache: false,
        data: "{ id: '" + id + "'}",
        success: function (result) {
            $("#defineStockWare").html(result);
        },
        error: function (jqXHR, exception) {
            console.log(jqXHR);
        }
    });
};

function dfnPrsnl() {
    var id = 1903;

    $.ajax({
        type: "GET",
        url: "/Admin/Add/DefinePrsnl/" + id,
        //contentType: "application/html charset=utf-8",
        dataType: "html",
        cache: false,
        data: "{ id: '" + id + "'}",
        success: function (result) {
            $("#dfnPrsnl").html(result);
        },
        error: function (jqXHR, exception) {
            console.log(jqXHR);
        }
    });

};