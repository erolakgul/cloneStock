$(document).ready(function () {
    firstFunction();
    FillGroupThree();

    $("*").bind("click");
    $("#loadWaiting2").hide();


    setInterval(function () {
        var _mail = $("#userNamed").text().trim();
        $.ajax({
            type: "POST",
            url: "/Users/Panel/CheckActive/",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            cache: false,
            data: "{ mail: '" + _mail + "'}",
            success: function (result) {
                   //isonline false a düşürüldüyse sayfayı refresh yapıp login ekranına gönderiyoruz
                if(result == false) {
                    swal("Güncelleme Yapılacaktır", "1 dk sonra log out olacaksınız.İşleminizi tamamlayınız ve 8 dk sonra tekrar login olabilirsiniz.", "warning");
                    setTimeout("window.open('/Users/Panel', '_self');", 55000);
                }
            },
            error: function (jqXHR, exception) {
                console.log(jqXHR);
            }
        });
    }, 60000);

    //setTimeout("window.open('/Users/Writer', '_self');", 300000);

    //300000 milliseconds = 300 seconds = 5 minutes

    //as 60000 milliseconds = 60 seconds = 1 minute.

    $("input#materialNumber").blur(function () {
        var matcode = $("input#materialNumber").val().trim();

        $.ajax({
            type: "POST",
            url: "/Users/Panel/GetMaterialName/",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            cache: false,
            data: "{ matcode: '" + matcode + "'}",
            success: function (result) {
                if (result != "False") {
                    $("#materialName > input").val(result);
                    $("#rsltMsg").text("");
                } else {
                    $("#materialName > input").val("");
                    $("#rsltMsg").text("\nMalzeme Kodu Sayım Listesinde Bulunamadı.\n Lütfen Malzeme Tanımını El ile Giriniz.");
                };
            },
            error: function (jqXHR, exception) {
                console.log(jqXHR);
            }
        });

    });

});


// material sayfasındaki looader ı kapatır
function waitingMat() {
    setTimeout(showPage, 1000);
    function showPage() {
        $("#loadWaiting2").removeClass("loader");
        $("#loadWaiting2 > img").remove();
    }
};
// loader ı kapatır
function waiting() {
    //console.log("içerde");
    $("div#loadWaiting").show();

};
function ResultAction() {
    // console.log("dışarda");
    $("div#loadWaiting").hide();
    fillDropDown();

};

function FailureAction() {
    $("div#loadWaiting").hide();
}

// users/material sayfasında sayım yapan kişinin deposunda olan ve sayfaması gereken malzemeleri getirir
function firstFunction() {
    /*var stockStore = $("#aktifDepo > select").find("option:selected").text();*/
    $("#showMaterialsForPersonel").show(1000);
    $("#loadWaiting").show();

    var id = $("#userNamed").text();//.split('.').join('ş');

    $.ajax({
        type: "POST",
        url: "/Users/Panel/ShowMaterials/" + id,
        contentType: "application/html; charset=utf-8",
        //dataType: "html",
        cache: false,
        data: "{ id: '" + id + "'}",
        success: function (result) {
            $("#viewMaterialX").html(result);
            $("#searchMaterial").hide();
            $("#loadWaiting").hide();

            fillDropDown();
        },
        error: function (jqXHR, exception) {
            console.log(jqXHR);
        }
    });

};

function fillDropDown() {
    var id = "";

    $.ajax({
        type: "POST",
        url: "/Users/Panel/SetStockStore/",
        //contentType: "application/json; charset=utf-8",
        dataType: "Html",
        cache: false,
        // data: "" ,//"{ storeNo: '" + id + "'}",
        success: function (result) {
            $("#dropdownlist").html(result);
        },
        error: function (jqXHR, exception) {
            console.log(jqXHR);
        }
    });

};

function FillGroupThree() {
    var name = $("#userNamed").text();
    var id = name.split('.').join('#');

    //// userhome index ten showteamgroup action ına istek yapıyoruz,dönen değer showTeamGroup un içerisine yazdırılacak
    $.ajax({
        type: "POST",
        url: "/Users/Processor/ShowEnterMaterialForm/" + id,
        contentType: "application/json; charset=utf-8",
        //dataType: "html",
        cache: false,
        data: "{ id: '" + id + "'}",
        success: function (result) {
            // json veri dönecek
            //team no
            $("#teamNo > input").val(result[0]);
            //group no
            $("#groupNo > input").val(result[1]);
            //pername
            $("#teamFriends").html(result[2]);
            //depo
            $("#aktifDepo > input").val(result[3]);
            //stok yeri
            $("#aktifStokYeri > input").val(result[4]);
        },
        error: function (jqXHR, exception) {
            console.log(jqXHR);
        }
    });
};


function addMat(id) {
    $("#warningIndis").css("display", "none");
    $("#submitDataRef").css("display", "none");
    $("#submitData").css("display", "block");

    //$("#indisData").removeAttr("disabled");
    $("#serialNumData").removeAttr("disabled");
    $("#specData").removeAttr("disabled");

    $("#gncl > h4").text("Ekle/Güncelle");
    $("#gncl > h4").css("color", "black");
    $("#submitData").removeAttr("disabled");

    $("#adetData").removeAttr("disabled");
    $('#showMaterialsForPer > tr.cls' + id).css("background-color", "aquamarine");

    localStorage.setItem("last-color", id);

    $("#InventId").text("");

    var _indis = $("#showMaterialsForPer > tr.cls" + id + " > td:nth-child(3)").text().trim();
    var _seriNo = $("#showMaterialsForPer > tr.cls" + id + " > td:nth-child(5)").text().trim();
    var _specData = $("#showMaterialsForPer > tr.cls" + id + " > td:nth-child(6)").text().trim();
    var _adet = $("#showMaterialsForPer > tr.cls" + id + " > td:nth-child(10)").text().trim();

    var _mCode = $("#showMaterialsForPer > tr.cls" + id + " > td:nth-child(2)").text().trim();
    var _stockStore = $("#showMaterialsForPer > tr.cls" + id + " > td:nth-child(8)").text().trim();
    var _stockPlace = $("#showMaterialsForPer > tr.cls" + id + " > td:nth-child(9)").text().trim();


    var _mType = $("#showMaterialsForPer > tr.cls" + id + " > td:nth-child(7)").text().trim();
    $("#mttype").text(_mType);

    $("#clsIndis").text(_indis);
    $("#specData").val(_specData);

    if (_indis == "*") {
        $("#indisData").val(_indis);
        if (_stockPlace != "107") { //son toplantı sonrası eklendi
            $("#indisData").attr("disabled", "disabled");
        }
    } else {
        $("#indisData").val("");
        $("#indisData").removeAttr("disabled");
    }

    // $("#serialNumData").val(_seriNo); istenirse açılır son toplantı da kapatıldı
    $("#serialNumData").val("");

    $("#adetData").val(_adet);
    $("#dbIdNumber").val(id);

    $("#mCode").val(_mCode);
    $("#store").val(_stockStore);
    $("#stockPlace").val(_stockPlace);

};


function delMat(id) {
    // userhome index ten showteamgroup action ına istek yapıyoruz,dönen değer showTeamGroup un içerisine yazdırılacak

    swal({
        title: "Emin misiniz?",
        text: "Malzeme Sayım Kaydı Silinecek!",
        type: "warning",
        showCancelButton: true,
        confirmButtonColor: "#DD6B55",
        confirmButtonText: "Evet, sil!",
        cancelButtonText: "Hayır, kalsın!",
        closeOnConfirm: false,
        closeOnCancel: false
    },
    function (isConfirm) {
        if (isConfirm) {
            $.ajax({
                type: "POST",
                //url: "Users/UserHome/Delete/" + id,
                url: "/Users/Panel/DelMaterial/" + id,
                //contentType: "application/json; charset=utf-8",
                dataType: "html",
                cache: false,
                data: "{ id: '" + id + "'}",
                success: function (result) {
                    swal("Silindi!", "Malzeme Sayım Kaydı Silindi.", "success");
                    $("#inventResult").html(result);
                    // a7();
                    firstFunction();
                },
                error: function (jqXHR, exception) {
                    var msg = '';
                    msg = 'Uncaught Error.\n' + jqXHR.responseText;
                    ////alert(msg);
                    console.log(jqXHR);
                }
            });
        } else {
            swal("İptal Edildi", "", "error");
        }
    });
};

//malzeme adedi güncelleme
function updMat(id) {
    //console.log("id :" + $("#showMaterialsForPer > tr.cls" + id + " > td:nth-child(5)").text().trim());
    $("#warningIndis").css("display", "none");
    $("#submitDataRef").css("display", "none");
    $("#submitData").css("display", "block");

    $("#indisData").attr("disabled", "disabled");
    $("#serialNumData").attr("disabled", "disabled");
    $("#specData").attr("disabled", "disabled");

    $("#gncl > h4").text("Ekle/Güncelle");
    $("#gncl > h4").css("color", "black");
    $("#submitData").removeAttr("disabled");

    if ($("#showMaterialsForPer > tr.cls" + id + " > td:nth-child(5)").text().trim().length > 0) {
        $("#gncl > h4").text("Seri numaralı malzeme güncellemesi yapılmamaktadır.\nGerekirse MALZEME EKLE seçeneğini kullanınız.");
        $("#gncl > h4").css("color", "red");
        $("#submitData").attr("disabled", "disabled");
    }


    var _teamNo = $("#teamNoTR").text().trim();
    var _inventId = "";

    if (_teamNo == "3") {
        _inventId = $("#showMaterialsForPer > tr.cls" + id + " > td:nth-child(13) > a").attr("id").trim();
    } else {
        _inventId = $("#showMaterialsForPer > tr.cls" + id + " > td:nth-child(12) > a").attr("id").trim();
    };

    $("#InventId").text(_inventId);

    // PARTİAL SHOW MATERIAL İÇİN
    //console.log("ii :" + id);
    var _indis = $("#showMaterialsForPer > tr.cls" + id + " > td:nth-child(3)").text().trim();
    var _seriNo = $("#showMaterialsForPer > tr.cls" + id + " > td:nth-child(5)").text().trim();
    var _specData = $("#showMaterialsForPer > tr.cls" + id + " > td:nth-child(6)").text().trim();
    var _adet = $("#showMaterialsForPer > tr.cls" + id + " > td:nth-child(10)").text().trim();

    var _mCode = $("#showMaterialsForPer > tr.cls" + id + " > td:nth-child(2)").text().trim();
    var _stockStore = $("#showMaterialsForPer > tr.cls" + id + " > td:nth-child(8)").text().trim();
    var _stockPlace = $("#showMaterialsForPer > tr.cls" + id + " > td:nth-child(9)").text().trim();

    $("#specData").val(_specData);
    $("#indisData").val(_indis);
    $("#serialNumData").val(_seriNo);
    $("#adetData").val(_adet);
    $("#dbIdNumber").val(id);
    // console.log("id :"+ id);
    $("#mCode").val(_mCode);
    $("#store").val(_stockStore);
    $("#stockPlace").val(_stockPlace);
};

function SaveData() {
    // renk değiştirme
    var id = localStorage.getItem("last-color");
    $('#showMaterialsForPer > tr.cls' + id).css("background-color", "none");

    if ($("#indisData").val().length == 0) {
        return swal("", "indis boş bırakılamaz", "warning");
    };

    var _valCss = $("#warningIndis").css("display");
    var _prmt = 0;

    if (_valCss == "none") {
        _prmt = 0; // sadece sayım tablosuna kaydettir
    } else if (_valCss == "block") {
        _prmt = 1; // hem sayım hem malzeme tablosuna kaydettir
    };

    var _adet = $("#adetData").val();
    if ((_adet == '') || _adet < 1) { //_adet == 0 ||

        alert('Miktar boş veya eksi olamaz');
        return 0;
    }

    var _inventId = $("#InventId").text();
    //console.log("_inventId :" + _inventId);

    var _specData = $("#specData").val();
    var _indis = $("#indisData").val();
    var _seriNo = $("#serialNumData").val();
    var _adet = $("#adetData").val();
    var id = $("#dbIdNumber").val();

    var _mcode = $("#mCode").val();
    var _stockStore = $("#store").val();
    var _stockPlace = $("#stockPlace").val();

    var _tmNo = $("#teamNoTR").text();
    var _grpNo = $("#groupNoTR").text();
    var _matType = $("#mttype").text();

    //alert(JSON.stringify(_collection));

    if (_inventId == null || _inventId == "") {

        var _collection = {
            "_id": id,
            "_matIndex": _indis,
            "_matSeriNum": _seriNo,
            "_matSpecial": _specData,
            "_quantity": _adet,
            "_teamNo": _tmNo,
            "_groupNo": _grpNo,
            "_mCode": _mcode,
            "_stockStore": _stockStore,
            "_stockPlace": _stockPlace,
            "_matType": _matType,
            "_warningNum": _prmt  //// 1 ise hem sayım hem malzeme tablosuna kaydettir 0 sa sadece sayım
        };


        $.ajax({
            type: "POST",
            url: "/Users/Panel/EnterMaterial/",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            cache: false,
            data: JSON.stringify(_collection),
            success: function (result) {

                $("#myModal").removeClass("in");
                $("#myModal").css("display", "none");
                $('#myModal').modal('toggle');

                var _dataXy = "";
                _dataXy = result.msg;

                var dbIdNumber = $("#dbIdNumber").val();
                //console.log("uyarı :" + _dataXy + "//" + result.msg);

                if (_dataXy == "UnAccepted") {
                    swal("Seri No Uyuşmuyor !", "Malzeme Ekle - sekmesini kullanarak malzemeyi kaydediniz,sonra sayım ekranında girişini yapınız..", "warning");
                    $(".cls" + dbIdNumber).css("background-color", "white");
                    return false;
                } else if (_dataXy == "SerNoBoş") {
                    swal("", "Malzemeye ait Seri No bilgisi Girilmemiş.", "warning");
                    $(".cls" + dbIdNumber).css("background-color", "white");
                    return false;
                } else if (_dataXy == "Sayılmış") {
                    swal("", "Malzeme Sayılmış", "warning");
                    $(".cls" + dbIdNumber).css("background-color", "white");
                    return false;
                } else if (_dataXy == "Have") {
                    swal("", "Malzeme Zaten Aynı Indis İle Kayıtlı", "warning");
                    $(".cls" + dbIdNumber).css("background-color", "white");
                    return false;
                } else if (_dataXy == "Başarısız") {
                    swal("", "Malzeme Kaydı Yapılamadı..", "warning");
                    $(".cls" + dbIdNumber).css("background-color", "white");
                    return false;
                } else if (_dataXy == "Boş") {
                    swal("", "Boş kayıtlar var.", "warning");
                    $(".cls" + dbIdNumber).css("background-color", "white");
                    return false;
                } else if (_dataXy == "Başarılı") {

                    if (_prmt == 1) {
                        // hem malzeme hem sayım tablosuna kaydetmiştir, sayım yaptığı kalemi beyaz hale getir
                        $(".cls" + dbIdNumber).css("background-color", "white");
                        swal("", "Yeni Indisiyle Malzeme Kaydedildi", "success");

                        GetMaterialCodeForInvestigate();
                        //window.location.href = "/Users/Writer";
                    } else if (_prmt == 0) {
                        $(".cls" + dbIdNumber).css("background-color", "lightgreen");

                        var adetData = $("#adetData").val().trim();

                        $("#matQ" + dbIdNumber).html(adetData);
                        $(".del" + dbIdNumber).removeAttr('style');
                        //$(".update" + dbIdNumber).removeAttr('style');
                        $(".add" + dbIdNumber).css('display', 'none');

                        var _serNo = $("#serialNumData").val();

                        $(".cls" + dbIdNumber + " > td:nth-child(5)").text(_serNo);
                        swal("", "Kaydedildi", "success");
                        //GetMaterialCodeForInvestigate();
                    };
                } else {
                    swal("", "beklenmedik hata..", "error");
                }
            },
            error: function (jqXHR, exception) {
                var msg = '';
                if (jqXHR.status === 0) {
                    msg = 'Not connect.\n Verify Network.';
                } else if (jqXHR.status == 404) {
                    msg = 'Requested page not found. [404]';
                } else if (jqXHR.status == 500) {
                    msg = 'Internal Server Error [500].';
                } else if (exception === 'parsererror') {
                    msg = 'Requested JSON parse failed.';
                } else if (exception === 'timeout') {
                    msg = 'Time out error.';
                } else if (exception === 'abort') {
                    msg = 'Ajax request aborted.';
                } else {
                    msg = 'Uncaught Error.\n' + jqXHR.responseText;
                }
                alert(msg);
                console.log(jqXHR);
            }
        });
    } else {

        var _collection = {
            "_id": _inventId,
            "_matIndex": _indis,
            "_matSeriNum": _seriNo,
            "_matSpecial": _specData,
            "_quantity": _adet,
            "_teamNo": _tmNo,
            "_groupNo": _grpNo,
            "_mCode": _mcode,
            "_stockStore": _stockStore,
            "_stockPlace": _stockPlace
        };

        $.ajax({
            type: "POST",
            url: "/Users/Panel/UpdateMaterial/",
            contentType: "application/json; charset=utf-8",
            dataType: "json",
            cache: false,
            data: JSON.stringify(_collection),
            success: function (result) {
                $("#myModal").removeClass("in");
                $("#myModal").css("display", "none");
                $('#myModal').modal('toggle');

                var _warrn = result.msg;

                //$(result).find("#viewbagMaterial").each(function (index) {
                //    _warrn = $(this).text();
                //});

                if (_warrn == "SingularApprove") {
                    swal("Güncellenemez", "Malzeme kontrolde onaylanmış", "error");
                } else if (_warrn == "Success") {
                    swal("", "Malzeme Güncellendi.", "success");
                } else if (_warrn == "Counted") {
                    swal("", "Malzeme Sayılmış.", "warning");
                } else if (_warrn == "Approved") {
                    swal("", "Malzeme Onaylanmış.", "warning");
                } else if (_warrn == "REDD") {
                    swal("Güncellenemez", "Malzeme Kontrolde Reddedilmiş.", "warning");
                } else if (_warrn == "PASSIVE") {
                    swal("Güncellenemez", "Malzeme Kontrolde 3.TAKIM a atanmış.", "warning");
                } else if (_warrn == "Unsuccess") {
                    swal("Güncellenemedi", "Birşeyler ters gitti.", "warning");
                }

                setTimeout("waits()", 2000);
            },
            error: function (jqXHR, exception) {
                console.log(jqXHR);
            }
        });
    }
};


function docSign(id) {

    swal({
        title: "Emin misiniz?",
        text: "Belgeye İmza Sonrası Girilemeyecek !",
        type: "warning",
        showCancelButton: true,
        confirmButtonColor: "#DD6B55",
        confirmButtonText: "İmzala!",
        cancelButtonText: "Vazgeç!",
        closeOnConfirm: false,
        closeOnCancel: false
    },
     function (isConfirm) {
         if (isConfirm) {

             // sayılmayan malzemeler var mı bunlar neler ?
             $.ajax({
                 type: "POST",
                 url: "/Users/Panel/CheckInventMaterial/" + id,
                 contentType: "application/json; charset=utf-8",
                 dataType: "json",
                 cache: false,
                 data: "{ id: '" + id + "'}",
                 success: function (result) {
                     var _ls = null;
                     var _mats = null;

                     var id = $("#userNamed").text().trim();
                     id = id;//.split('.').join('ğ');

                     //swal.close();
                     $("#rsltMsg > tr").remove();

                     var _collection = {
                         "_material": [],
                         "_id": id
                     };

                     $.each(result, function (index, element) {
                         _ls = "<tr> <td>" + element.MatCode + "</td>" + "<td>" + element.MatIndex + "</td>" + "<td>" + element.MatSection + "</td>" + "<td>" + element.MatSectionPlace + "</td>" + "<td>" + element.MatSerialNumber + "</td>" + "<td>" + element.MatSpecialStock + "</td></tr>";
                         //$("#missionPlace").append(_ls);

                         _mats = _mats + " # " + element.MatCode + " # " + element.MatIndex + " # " + element.MatSection + " # " + element.MatSectionPlace + " \n";
                         $("#Warningmessage").show();
                         $('#Warningmessage').dialog("open");
                         $("#rsltMessagess").append(_ls);

                         _collection._material.push({ "_code": element.MatCode, "_index": element.MatIndex, "_depo": element.MatSection, "_stokYeri": element.MatSectionPlace, "_specStock": element.MatSpecialStock, "_serNo": element.MatSerialNumber });
                     });

                     swal({
                         title: "Kaydedilmeyen Malzemeler Olabilir.",
                         text: "Sıfır Miktar Olarak Kaydetmeyi Kabul Ediyor Musunuz ?",
                         type: "warning",
                         showCancelButton: true,
                         confirmButtonColor: '#DD6B55',
                         confirmButtonText: "EVET",
                         cancelButtonText: "HAYIR",
                         closeOnConfirm: false,
                         closeOnCancel: false
                     },
                      function (isConfirm) {
                          if (isConfirm) {
                              //$('#train').on('click', trainClick);
                              //$('#train').off('click', trainClick);
                              $("*").unbind("click");

                              $.ajax({
                                  url: "/Users/Panel/DocSigns/" + id,
                                  type: "POST",
                                  data: "{ id: '" + id + "'}",//JSON.stringify(_collectionA),
                                  //contentType: "application/json; charset=utf-8",
                                  dataType: "json",
                                  //cache: false,
                                  success: function (result) {
                                      // similar behavior as an HTTP redirect
                                      //$("*").bind("click");

                                      if (result == true) {
                                          swal({
                                              title: "",
                                              text: "Belge İmzalanmıştır.",
                                              timer: 2000,
                                              type: "success",
                                              showConfirmButton: false
                                          },
                                            function () {
                                                window.location.replace("/");
                                            }
                                          );
                                      } else if (result == false) {
                                          //window.location.replace("/Users/Writer");
                                          //$("*").bind("click");
                                          swal("", "Birşeyler ters gitti..", "error");
                                          firstFunction();
                                      };

                                      //window.location.href = "http://stackoverflow.com";
                                  },
                                  error: function (jqXHR, exception) {
                                     // $("*").bind("click");
                                      var msg = '';
                                      console.log(jqXHR);
                                  }
                              });

                          } else {
                              swal("", "Vazgeçildi", "error");
                          };

                      })
                 },
                 error: function (jqXHR, exception) {
                     var msg = '';
                     if (jqXHR.status === 0) {
                         msg = 'Not connect.\n Verify Network.';
                     } else if (jqXHR.status == 404) {
                         msg = 'Requested page not found. [404]';
                     } else if (jqXHR.status == 500) {
                         msg = 'Internal Server Error [500].';
                     } else if (exception === 'parsererror') {
                         msg = 'Requested JSON parse failed.';
                     } else if (exception === 'timeout') {
                         msg = 'Time out error.';
                     } else if (exception === 'abort') {
                         msg = 'Ajax request aborted.';
                     } else {
                         msg = 'Uncaught Error.\n' + jqXHR.responseText;
                     }
                     //alert(msg);
                     console.log(jqXHR);
                 }
             });
             ///
         } else {
             swal("İptal Edildi", "", "error");
         };
     });
};

function IsSign() {
    var id = $("#userNamed").text().trim();

    $.ajax({
        type: "POST",
        url: "/Users/Panel/IsSign/" + id,
        contentType: "application/json charset=utf-8",
        dataType: "json",
        cache: false,
        data: "{ id: '" + id + "'}",//JSON.stringify(_collect),
        success: function (result) {
            if (result == true) {

                docSign(id);

            } else {
                swal("", "Henüz imzalamanız için yetki verilmemiştir.", "error");
                setInterval(function () {
                    window.location.replace("/");
                }, 4000); // 2sn
            };

        },
        error: function (jqXHR, exception) {
            console.log(jqXHR);
        }
    });

};