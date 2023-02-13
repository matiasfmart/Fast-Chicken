var orderNum = 0;
var total;

function Danger(msg) {
    $("#msg-container").html("<div id='msg'><div class='error-msg'><i class='bi bi-x-circle'></i> " + msg + "</div></div>");
}

function FinishDay() {
//    orderNum = 0;
}

function FinishOrder() {
    debugger
    $('#orderView').load('/Home/FinishOrder', { items: orderItems, total: total, orderNum: orderNum }, function (response, status, xhr) {
        debugger
        if (status == 'success') {

            console.log(orderItems);
            console.log(orderNum);
            console.log(total);

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
    let drink;
    let ice;
    let side;
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

    if ($('input[type="radio"][name="ice"][id=con]').prop('checked')) {
        ice = true;
    } else if ($('input[type="radio"][name="ice"][id=sin]').prop('checked')) {
        ice = false
    } else {
        return false;
    }

    var obj = {     
        ComboId: id,
        OrderItemId : orderItemId,
        Name: name,
        Price: price,
        Side: side,
        Drink: drink,
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

//function ModalUpdateItem(id) {
//    const indexItem = orderItems.findIndex((e) => e.OrderItemId == id);
//    orderItems[indexItem].count = orderItems[indexItem].count + 1;
//}