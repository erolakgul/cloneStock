$(document).ready(function () {

    setInterval(function () {
        var _mail = $("#userNamed").text().trim();
        $.ajax({
            type: "POST",
            url: "/AreaController/Panel/CheckActive/",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            cache: false,
            data: "{ mail: '" + _mail + "'}",
            success: function (result) {
                // isonline false a düşürüldüyse sayfayı refresh yapıp login ekranına gönderiyoruz
                if (result == false) {
                    swal("Güncelleme Yapılacaktır", "1 dk sonra log out olacaksınız.İşleminizi tamamlayınız ve 8 dk sonra tekrar login olabilirsiniz.", "warning");
                    setTimeout("window.open('/Users/Panel', '_self');", 55000);
                }
            },
            error: function (jqXHR, exception) {
                console.log(jqXHR);
            }
        });
    }, 60000);

});

function getPersonel() {
    var id = 1903;

    $.ajax({
        type: "POST",
        url: "/AreaController/Panel/GetPersonel/" + id,
        contentType: "application/json charset=utf-8",
        dataType: "html",
        cache: false,
        data: "{ id: '" + id + "'}",//JSON.stringify(_collect),
        success: function (result) {
            $("#fillSignRole").html(result);

        },
        error: function (jqXHR, exception) {
            console.log(jqXHR);
        }
    });
};

function wareApprovePage() {
    var id = 1903;

    $.ajax({
        type: "GET",
        url: "/AreaController/Panel/OpenAppPage/" + id,
        contentType: "application/json charset=utf-8",
        dataType: "html",
        cache: false,
        data: "{ id: '" + id + "'}",//JSON.stringify(_collect),
        success: function (result) {
            $("#fillApp").html(result);

            var _depo = localStorage.getItem("_ware");
            var _stock = localStorage.getItem("_stock");

            if (_depo != null && _stock != null) {
                $("#stockStoreSelect > select > option:selected").text(_depo);
                $("#stockPlaceSelect > select > option:selected").text(_stock);
                $("#stockEndPlaceSelect > select > option:selected").text(_stock);
            };
        },
        error: function (jqXHR, exception) {
            console.log(jqXHR);
        }
    });
};

function FillStockPlace() {
    var storeNo = $("#stockStoreSelect > select").find("option:selected").text();
    var id = storeNo;

    $.ajax({
        type: "POST",
        url: "/AreaController/Panel/SelectStockPlace/" + id,
        contentType: "application/json; charset=utf-8",
        //dataType: "html",
        cache: false,
        data: "{ id: '" + id + "'}",
        success: function (result) {
            //stok yeri ile ilgili combobox doldurulacak
            $("#stockPlaceSelect > select").empty();
            var listItems;
            for (var i = 0; i < result.length; i++) {
                listItems += "<option value='" + result[i] + "'>" + result[i] + "</option>";
            }
            $("#stockPlaceSelect > select").html(listItems);
            $("#stockEndPlaceSelect > select").html(listItems);
        },
        error: function (jqXHR, exception) {
            console.log(jqXHR);
        }
    });

};

function FillStockPlaceX() {
    var storeNo = $("#stockStoreSelectX > select").find("option:selected").text();
    var id = storeNo;

    $.ajax({
        type: "POST",
        url: "/AreaController/Panel/SelectStockPlace/" + id,
        contentType: "application/json; charset=utf-8",
        //dataType: "html",
        cache: false,
        data: "{ id: '" + id + "'}",
        success: function (result) {
            //stok yeri ile ilgili combobox doldurulacak
            $("#stockPlaceSelectX > select").empty();
            var listItems;
            for (var i = 0; i < result.length; i++) {
                listItems += "<option value='" + result[i] + "'>" + result[i] + "</option>";
            }
            $("#stockPlaceSelectX > select").html(listItems);
            $("#stockEndPlaceSelectX > select").html(listItems);
        },
        error: function (jqXHR, exception) {
            console.log(jqXHR);
        }
    });

};

function MatchStockPlace() {
    var placeNo = $("#stockPlaceSelect > select").find("option:selected").text();
    $("#stockEndPlaceSelect > select > option:selected").text(placeNo);

    var placeNoX = $("#stockPlaceSelectX > select").find("option:selected").text();
    $("#stockEndPlaceSelectX > select > option:selected").text(placeNoX);
};

function wareApproveMaterialPage() {
    var id = 1903;

    $.ajax({
        type: "GET",
        url: "/AreaController/Panel/OpenAppMaterialPage/" + id,
        contentType: "application/json charset=utf-8",
        dataType: "html",
        cache: false,
        data: "{ id: '" + id + "'}",//JSON.stringify(_collect),
        success: function (result) {
            $("#fillAppMat").html(result);

            var _depo = localStorage.getItem("_ware");
            var _stock = localStorage.getItem("_stock");

            if (_depo != null && _stock != null) {
                $("#stockStoreSelect > select > option:selected").text(_depo);
                $("#stockPlaceSelect > select > option:selected").text(_stock);
                $("#stockEndPlaceSelect > select > option:selected").text(_stock);
            };


        },
        error: function (jqXHR, exception) {
            console.log(jqXHR);
        }
    });
};

