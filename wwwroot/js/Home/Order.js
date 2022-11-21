var orderNum = 0;
var total;

function FinishDay() {
    orderId = 0;
}

function FinishOrder() {
    debugger
    $('#orderView').load('/Home/FinishOrder', { items: orderItems, total: total, orderNum: orderNum }, function (response, status, xhr) {
        if (status == 'success') {
            orderItems = [];
            orderItemId = 0;
            orderNum++;
        }
    })
}

function NewOrderBtn() {
    $('#orderView').load('/Home/Order', { items: orderItems }, function (response, status, xhr) {
        if (status == 'success') {
            LoadCombosList();
        }
    })
    //$.post('/Home/Order')
    //.done(function (data) {

    //})
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
    } else {
        drink = "Jugo";
    }

    if ($('input[type="radio"][name="side"][id=arroz]').prop('checked')) {
        side = "Arroz";
    } else if ($('input[type="radio"][name="drink"][id=ensalada]').prop('checked')) {
        side = "Ensalada";
    } else {
        side = "Papas";
    }

    ice = $('input[type="radio"][name="ice"][id=con]').prop('checked');

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
    $('#order_items').load('/Home/AddOrderItems', { items: items }, function (response, status, xhr) {
        if (status == 'success') {
            $('#dialog').modal('hide');
        }
    })
}

var orderItems = [];
function AddItem(id, name, price, type) {
    orderItems.push(GetOrderItem(id, name, price, type));
    AddOrderItems(orderItems);
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