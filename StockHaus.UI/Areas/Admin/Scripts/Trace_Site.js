$(document).ready(function () {


});

////////////////////////////////////////////////////////////////// TRACE CONTROLLER ///////////////////////////////////////

/////////////////////////////////////  sayım log ları ///////////////////
function inventLog() {
    var id = 1;

    $.ajax({
        type: "GET",
        url: "/Admin/Trace/InventoryLog/" + id,
        //contentType: "application/html charset=utf-8",
        dataType: "html",
        cache: false,
        data: "{ id: '" + id + "'}",
        success: function (result) {
            $("#showLog").html(result);
        },
        error: function (jqXHR, exception) {
            console.log(jqXHR);
        }
    });

};

/////////////////////////////////////   sayım tamamlanma yüzdeleri ///////////////////////
function showPercentResult() {
    $.ajax({
        type: "POST",
        url: "/Admin/Trace/TraceStockCount/",
        //contentType: "application/json; charset=utf-8",
        dataType: "html",
        cache: false,
        //data: "{ id: '" + id + "'}",
        success: function (result) {
            $("#showCompletePercent").html(result);
        },
        error: function (jqXHR, exception) {
            console.log(jqXHR);
        }
    });
};
////////////////////////////////  ONAYLANMIŞ MALZEMELER  /////////////////////////////////


// onaylanmış malzemeleri listeler
function showApproveMaterial() {

    var _mat = $("#mat").val();
    var _matSec = $("#matSec").val();
    var _matSecPlace = $("#matSecPla").val();

    var _IsInclude = "";
    if ($("#isInclude").prop('checked') == true) {
        _IsInclude = 1;
    } else {
        _IsInclude = 0;
    }

    var _collect = {
        "_matCode": _mat,
        "_matSection": _matSec,
        "_matSectionPlace": _matSecPlace,
        "_IsInclude": _IsInclude
    };

    var id = 1;

    $.ajax({
        type: "GET",
        url: "/Admin/Trace/InventApprovedMat/" + id,
        contentType: "application/json charset=utf-8",
        dataType: "html",
        cache: false,
        data: "{ id: '" + id + "'}",//JSON.stringify(_collect),
        success: function (result) {
            $("#rsltApprovedMat").html(result);
        },
        error: function (jqXHR, exception) {
            console.log(jqXHR);
        }
    });
};

////////////////////////////////////  depo sayım onaylama ekranı ///////////////////////////////

function wareApprovePage() {
    var id = 1903;

    $.ajax({
        type: "GET",
        url: "/Admin/Trace/OpenAppPage/" + id,
        contentType: "application/json charset=utf-8",
        dataType: "html",
        cache: false,
        data: "{ id: '" + id + "'}",//JSON.stringify(_collect),
        success: function (result) {
            $("#fillApp").html(result);
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
        url: "/Admin/Trace/SelectStockPlace/" + id,
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
        },
        error: function (jqXHR, exception) {
            console.log(jqXHR);
        }
    });

};


function MatchStockPlace() {
    var placeNo = $("#stockPlaceSelect > select").find("option:selected").text();
    $("#stockEndPlaceSelect > select > option:selected").text(placeNo);
};