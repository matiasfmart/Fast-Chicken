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

function LoadComboModal(name, type, price){
    $('#dialog').load('/Home/LoadComboModal', { name: name, type: type, price: price }, function (response, status, xhr) {
        if (status == 'success') {
            $('#dialog').modal('show');
        }
    })
}

function GetOrderItem(name, price, type) {
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

var orderItems = [];
function AddOrderItem(name, price, type) {
    orderItems.push(GetOrderItem(name, price, type));
    debugger
    $('#order_items').load('/Home/AddOrderItem', { items: orderItems }, function (response, status, xhr) {
        if (status == 'success') {
            $('#dialog').modal('hide');
        }
    })
}