$(document).ready(function () {

    if ($("#userNamed").text().length == 0) {
        window.location.replace("/");
    };

    //$("img.peruser").effect("highlight", {}, 3000);
    //$("img.peruser").fadeIn(1000).fadeOut(2000);
    setInterval(function () {
        for (var i = 0; i < 100; i++) {
           // $('img.peruser' + i).toggle();
            $('img.peruser' + i).fadeToggle("slow", "linear");
        }
    }, 750);
});

function checkStatusOfPersonel() {
    
    $.ajax({
        type: "GET",
        url: "/Admin/Panel/GetOnlineUsers/",
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        cache: false,
        //data: "{ id: '" + id + "'}",
        success: function (result) {
            $("#getPersonelStatus").html(result);
        },
        error: function (jqXHR, exception) {
            console.log(jqXHR);
        }
    });
};


// tab a tıklanınca partial view ü getirir
function createTeam() {
    $.ajax({
        type: "POST",
        url: "/Admin/Panel/GetCreateTeamPage/",
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        cache: false,
        //data: "{ id: '" + id + "'}",
        success: function (result) {
            $("#ctInterface").html(result);
            $("#teamselect > select > option").each(function () {
                $(this).attr("value", $(this).text());   // this.value
            });
        },
        error: function (jqXHR, exception) {
            console.log(jqXHR);
        }
    });
};

// team seçildikten sonra sıradaki grup no bilgisi çekilir
function selectLastGroupNo() {
    //var _text = $("#teamselect > select").find("option:selected").val();
    var _text = $("#teamselect > select > option:selected").text();
    var id = "";

    if (_text == "Birinci") {
        id = 1;
    } else if (_text == "İkinci") {
        id = 2;
    } else if (_text == "Üçüncü") {
        id = 3;
    };

    $.ajax({
        type: "POST",
        url: "/Admin/Panel/LastGroupNoForTeam/" + id,
        //contentType: "application/json; charset=utf-8",
        dataType: "html",
        cache: false,
        //data: "{ id: '" + id + "'}",
        success: function (result) {

            if (result == 0) {
                $("#enterGroupNo > input").val(1);
            } else {
                $("#enterGroupNo > input").val(result);
            }
        },
        error: function (jqXHR, exception) {
            console.log(jqXHR);
        }
    });
};

///////////////////////////////////////////// create team partial
function getEventTarget(e) {
    e = e || window.event;
    return e.target || e.srcElement;
};

setInterval(function () {
    sid = $("#personalName").val();
    if (sid == null || sid == "") {
        $("#estimatedPersonalName > li").remove();
    }
}, 1000);

function GetPersonalName() {
    var id = '';
    if ($("#personalName").val() != id) {
        id = $("#personalName").val();

        $.ajax({
            type: "POST",
            url: "/Admin/Panel/PreEstimatedPerName/" + id,
            //contentType: "application/json; charset=utf-8",
            dataType: "Json",
            cache: false,
            data: "{ id: '" + id + "'}",
            success: function (result) {
                $("#estimatedPersonalName > li").remove();
                $.each(result, function (index, element) {
                    var $newPerName = $("#estimatedPersonalName").append($('<li>', {
                        text: element._name
                    }));

                    $newPerName.show('slow');
                });
            },
            error: function (jqXHR, exception) {
                console.log(jqXHR);
            }
        });
    }
};
///////////////////////////////////////////// create team partial END

////////////////////////// view team

function viewTeam() {
    var id = 1903;
    //// userhome index ten showteamgroup action ına istek yapıyoruz,dönen değer showTeamGroup un içerisine yazdırılacak
    $.ajax({
        type: "POST",
        url: "/Admin/Panel/ShowTeamGroup/" + id,
        //contentType: "application/json; charset=utf-8",
        dataType: "html",
        cache: false,
        data: "{ id: '" + id + "'}",
        success: function (result) {
            $("#getGroupTeam").html(result);
        },
        error: function (jqXHR, exception) {
            console.log(jqXHR);
        }
    });
};

function sil(id) {
    // userhome index ten showteamgroup action ına istek yapıyoruz,dönen değer showTeamGroup un içerisine yazdırılacak

    swal({
        title: "Emin misiniz?",
        text: "Personel Gruptan Silinecek!",
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
                url: "/Admin/Panel/Delete/" + id,
                //contentType: "application/json; charset=utf-8",
                dataType: "Json",
                cache: false,
                data: "{ id: '" + id + "'}",
                success: function (result) {
                    if (result == 'Sayım') {
                        swal("Personel Silinemiyor!", "İş Başındaki Personel Silinemez", "error");
                    } else if (result == "Silindi") {
                        swal("Silindi!", "Personel Gruptan Çıkarıldı.", "success");
                        $("#getGroupTeam").html(result);
                        viewTeam();
                    } else if (result == "Hata") {
                        swal("", "Personel Rolü Değiştirilemedi", "error");
                    } else if (result == "IGError") {
                        swal("", "Personel Gruplardan Pasife çekilemedi !", "error");
                    }
                },
                error: function (jqXHR, exception) {
                    var msg = '';
                    msg = 'Uncaught Error.\n' + jqXHR.responseText;
                    //alert(msg);
                    console.log(jqXHR);
                }
            });
        } else {
            swal("İptal Edildi", "", "error");
        }
    });
};

///////////////////////////////// ASSIGN TEAM ////////////////////////
function assignTeam() {
    $.ajax({
        type: "POST",
        url: "/Admin/Panel/GetAssignTeamPage/",
        contentType: "application/json; charset=utf-8",
        dataType: "html",
        cache: false,
        //data: "{ id: '" + id + "'}",
        success: function (result) {
            $("#atInterface").html(result);

            $("#teamselect > select > option").each(function () {
                $(this).attr("value", $(this).text());   // this.value
            });

            FillText();
        },
        error: function (jqXHR, exception) {
            console.log(jqXHR);
        }
    });
};

function FillText() {

    var _text = $("#teamselect2 > select option:checked").val();
    var id = _text;

    var id2 = $("#groupsel2 > select").val();// > option:selected").text();
    localStorage.setItem("selectedGroupNo", id2);

    if (id2 == "" || id2 == null) {
        id2 = "1";
    };

    //console.log("ddd :" + id2);
    $.ajax({
        type: "POST",
        url: "/Admin/Panel/SelectGroupPersonel/" + id + id2,
        contentType: "application/json; charset=utf-8",
        //dataType: "html",
        cache: false,
        data: "{ id: '" + id + "-" + id2 + "'}",
        success: function (result) {
            $("#perNameOne > input").val(result[0]);
            $("#perNameTwo > input").val(result[1]);

            missionAssignedBefore();
        },
        error: function (jqXHR, exception) {
            console.log(jqXHR);
        }
    });
};

// görevli olduğu yerleri yerleştir ekrana

function missionAssignedBefore() {
    var _userName = $("#perNameOne > input").val();
    $("#userNamedMission").text(_userName); // partialAssignTask => table span

    $.ajax({
        type: "POST",
        url: "/Admin/Panel/CheckedMissionPlace/" + _userName,
        //contentType: "application/json; charset=utf-8",
        dataType: "Json",
        cache: false,
        data: "{ id: '" + _userName + "'}",
        success: function (result) {
            $("#missionPlace > tr").remove();
            // partialassigntask
            $.each(result, function (index, element) {
                var _ls = "<tr> <td>" + element._ware + "</td>" + "<td>" + element._stock + "</td>" + "<td>" + element._endStockPlace + "</td></tr>";
                $("#missionPlace").append(_ls);
            });
            proc();
        },
        error: function (jqXHR, exception) {
            var msg = '';
            msg = 'Uncaught Error.\n' + jqXHR.responseText;
            console.log(jqXHR);
        }
    });
    //proc();
};

function proc() {
    //var _text = $("#teamselect > select").find("option:selected").text();
    var _text = $("#teamselect2 > select").val();// > option:selected").text();
    var id = _text; // _text.substring(7, 30);
    //console.log("proc :" + _text);

    $.ajax({
        type: "POST",
        url: "/Admin/Panel/SetGroupNoJS/" + id,
        contentType: "application/json; charset=utf-8",
        //dataType: "html",
        cache: false,
        data: "{ id: '" + id + "'}",
        success: function (result) {
            $("#groupsel2 > select > option").remove();
            var _selected = localStorage.getItem("selectedGroupNo");

            $.each(result, function (index, element) {
                var _ks = "<option>" + element + "</option>";
                $("#groupsel2 > select").append(_ks);
            });
            $("#groupsel2 > select").val(_selected);
        },
        error: function (jqXHR, exception) {
            console.log(jqXHR);
        }
    });
}

function FillStockPlace() {
    var storeNo = $("#stockStoreSelect > select").find("option:selected").text();
    var id = storeNo;

    $.ajax({
        type: "POST",
        url: "/Admin/Panel/SelectStockPlace/" + id,
        contentType: "application/json; charset=utf-8",
        //dataType: "html",
        cache: false,
        data: "{ id: '" + id + "'}",
        success: function (result) {
            //stok yeri ile ilgili combobox doldurulacak
            $("#stockPlaceSelect > select").empty();
            $("#stockPlaceEndSelect > select").empty();

            var listItems;
            for (var i = 0; i < result.length; i++) {
                listItems += "<option value='" + result[i] + "'>" + result[i] + "</option>";
            }
            $("#stockPlaceSelect > select").html(listItems);

            $("#stockPlaceEndSelect > select").html(listItems);
        },
        error: function (jqXHR, exception) {
            console.log(jqXHR);
        }
    });
};

function matchMatSection() {
    var storeNo = $("#stockPlaceSelect > select").find("option:selected").text();
    var id = storeNo;

    $("#stockPlaceEndSelect > select").find("option:selected").text(id);
    $("#stockPlaceEndSelect > select").find("option:selected").val(id);
};

///////////////////////////////// ASSIGN TEAM END

