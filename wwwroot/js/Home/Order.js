var orderNum = 0;
var total;

function Danger(msg) {
    $("#msg-container").html("<div id='msg'><div class='error-msg'><i class='bi bi-x-circle'></i> " + msg + "</div></div>");
}

function Warning(msg) {
    $("#msg-container").html("<div id='msg'><div class='alert-msg'><i class='bi bi-x-circle'></i> " + msg + "</div></div>");
}

function ClearMessage() {
    $("#msg-container").html("");
}

function FinishDay() {
//    orderNum = 0;
}

function FinishOrder() {
    debugger
    let hereToGo = $('input[type="radio"][name="delivery"][id=ToGo]').prop('checked') ? true : false;
    $('#orderView').load('/Home/FinishOrder', { items: orderItems, total: total, orderNum: orderNum, hereToGo: hereToGo }, function (response, status, xhr) {
        debugger
        if (status == 'success') {

            $('#dialog').modal('hide');
            orderItems = [];
            orderItemId = 0;
            orderNum++;

            // cliente
            $('#cocina').hide();
            $('#cliente').show();
            window.print();

            // Cocina
            $('#cliente').hide();
            $('#cocina').show();
            window.print();
        }
    })
}

function ConfirmFinishOrder() {
    debugger
    if (orderItems.length != 0) {
        $('#dialog').load('/Home/ConfirmFinishOrder', function (response, status, xhr) {
            if (status == 'success') {
                $('#dialog').modal('show');
            }
        })
    } else {
        Danger("No hay combos agregados.")
    }
}

function LoadCombosList() {
    $('#combos_list').load('/Home/GetCombos', function () {
        if (status == 'success') {
        }
    })
}

//$(document).ready(function () {
//    LoadCombosList();
//})

document.addEventListener('DOMContentLoaded', function (event) {
    LoadCombosList();
})

function LoadComboModal(id, name, type, price) {
    var combo = GetCombo(id, name, type, price);
    $('#dialog').load('/Home/LoadComboModal', { combo: combo }, function (response, status, xhr) {
        if (status == 'success') {
            $('#dialog').modal('show');
        }
    })
}

function GetCombo(id, name, type, price) {
    var obj = {
        ComboId: id,
        Name: name,
        Price: price,
        Type: type
    }
    return obj
}

var orderItemId = 0;
function GetOrderItem(id, name, price, type) {
    orderItemId++;
    let drink=-1;
    let ice=-1;
    let side=-1;
    let idDrink = -1;
    let idSide = -1;
    let quantity = 0;
    let count = 0;

    count = 0;
    $('input[type="radio"][name="drink"]').each(function () {
        count++;
        if ($(this).prop("checked")) {
            drink = $(this).data("name");
            idDrink = $(this).data("id");
            quantity = $(this).data("quantity");
        }
    });

    if (count != 0 && drink == -1) return false;
    else if (count != 0  && quantity <= 0) {
        Warning("El producto esta sin stock");
    }

    if (count > 0) {   // Si tiene bebida aplica con hielo o sin hielo
        if ($('input[type="radio"][name="ice"][id=con]').prop('checked')) {
            ice = true;
        } else if ($('input[type="radio"][name="ice"][id=sin]').prop('checked')) {
            ice = false
        } else {
            return false;
        }
    }

    count = 0;
    $('input[type="radio"][name="side"]').each(function () {
        count++;
        if ($(this).prop("checked")) {
            side = $(this).data("name");

            idSide = $(this).data("id");
            quantity = $(this).data("quantity");
        }
    });

    if (count != 0 && side == -1) return false; 
    else if (quantity <= 0) {
        Warning("El producto esta sin stock");
    }
    /*
    if ($('input[type="radio"][name="drink"][id=sprite]').prop('checked')) {
        drink = "Sprite";
    } else if ($('input[type="radio"][name="drink"][id=coca]').prop('checked')) {
        drink = "Coca";
    } else if ($('input[type="radio"][name="drink"][id=fanta]').prop('checked')) {
        drink = "Fanta";
    } else if ($('input[type="radio"][name="drink"][id=jugo]').prop('checked')) {
        drink = "Jugo";
    } else {
        return false;
    }

    if (type == "PO") {
        if ($('input[type="radio"][name="side"][id=arroz]').prop('checked')) {
            side = "Arroz";
        } else if ($('input[type="radio"][name="side"][id=ensalada]').prop('checked')) {
            side = "Ensalada";
        } else if ($('input[type="radio"][name="side"][id=papas]').prop('checked')) {
            side = "Papas";
        } else {
            return false;
        }
    }
    */

    
    var obj = {     
        ComboId: id,
        OrderItemId : orderItemId,
        Name: name,
        Price: price,
        Side: side,
        Drink: drink,
        IdSide: idSide,
        IdDrink: idDrink,
        Ice: ice,
        Type: type
    }
    //if ($('input[type="radio"][name="ice"][id=con]').prop('checked')) {
    //    ice = true;
    //} else {
    //    ice = false
    //}
    return obj;
}

function AddOrderItems(items) {
    items.length == 0 ? $('#btnFinishOrder').prop('disabled', true) : $('#btnFinishOrder').prop('disabled', false);
    $('#order_items').load('/Home/AddOrderItems', { items: items }, function (response, status, xhr) {
        if (status == 'success') {
            $('#dialog').modal('hide');
        }
    })
}

var orderItems = [];
function AddItem(id, name, price, type) {
    
    var item = GetOrderItem(id, name, price, type);
    if (item) {
        orderItems.push(item);
        AddOrderItems(orderItems);
    } else {
        Danger("El menu esta incompleto.")
    }
}

function DeleteItem(id) {
    debugger
    orderItems = orderItems.filter(element => element.OrderItemId != id)
    AddOrderItems(orderItems);
}

function ClearItems() {
    orderItems = [];
    orderItemId = 0;
    AddOrderItems(orderItems);
}

function changeSide(id) {
    if ($("#" + id).data('quantity') <= 0) {
        Warning("No hay stock de la guarnición seleccionada");
    } else {
        ClearMessage();
    }
}

function changeDrink(id) {
    if ($("#" + id).data('quantity') <= 0) {
        Warning("No hay stock de la bebida seleccionada");
    } else {
        ClearMessage();
    }
}

//function ModalUpdateItem(id) {
//    const indexItem = orderItems.findIndex((e) => e.OrderItemId == id);
//    orderItems[indexItem].count = orderItems[indexItem].count + 1;
//}