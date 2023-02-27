function LoadBebidas() {
    $('#ListaBebidas').load('/Admin/ListaBebidas', function () {
        if (status == 'success') {
        }
    })
}

function LoadGuarniciones() {
    $('#ListaGuarniciones').load('/Admin/ListaGuarniciones', function () {
        if (status == 'success') {
        }
    })
}

function LoadProductos() {
    $('#ListaProductos').load('/Admin/ListaProductos', function () {
        if (status == 'success') {
        }
    })
}

function LoadCombos() {
    $('#ListaCombos').load('/Admin/ListaCombos', function () {
        if (status == 'success') {
        }
    })
}

$('#btnActualizarBebidas').click(function (e) {

    var ids = [];

    $("div[id^='quantity_']").each(function (item, index) {
        ids.push({
            idDrink: $(this).data("id"),
            Name: "",
            Price: 0.0,
            Quantity: $(this).data("quantity")
        });
    });

    $.ajax({
        url: '/api/Admin/ActualizaStockBebidas',
        type: 'POST',
        dataType: "JSON",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify(ids)
    }).done(function (data) {
        if (data.Status == "OK") {
            LoadBebidas();
        }
    });

});

$('#btnDeshacerBebidas').click(function (e) {
    LoadBebidas();
});

$('#btnActualizarGuarniciones').click(function (e) {

    var ids = [];

    $("div[id^='quantity_']").each(function (item, index) {
        ids.push({
            idSide: $(this).data("id"),
            Name: "",
            Price: 0.0,
            Quantity: $(this).data("quantity")
        });
    });

    $.ajax({
        url: '/api/Admin/ActualizaStockGuarniciones',
        type: 'POST',
        dataType: "JSON",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify(ids)
    }).done(function (data) {
        if (data.Status == "OK") {
            LoadGuarniciones();
        }
    });

});

$('#btnDeshacerGuarniciones').click(function (e) {
    LoadGuarniciones();
});

$('#btnActualizarProductos').click(function (e) {

    var ids = [];

    $("div[id^='quantity_']").each(function (item, index) {
        ids.push({
            idProduct: $(this).data("id"),
            Name: "",
            Price: 0.0,
            Quantity: $(this).data("quantity")
        });
    });

    $.ajax({
        url: '/api/Admin/ActualizaStockProductos',
        type: 'POST',
        dataType: "JSON",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify(ids)
    }).done(function (data) {
        if (data.Status == "OK") {
            LoadProductos();
        }
    });

});

$('#btnDeshacerProductos').click(function (e) {
    LoadProductos();
});

function Quitar(id) {
    var quantity = parseInt($('#quantity_' + id).data('quantity'));

    var value = parseInt($('#value_' + id).val());

    $('#quantity_' + id).html(quantity - value);
    $('#quantity_' + id).data('quantity', quantity - value);
}

function Agregar(id) {

    var quantity = parseInt($('#quantity_' + id).data('quantity'));

    var value = parseInt($('#value_' + id).val());

    $('#quantity_' + id).html(quantity + value);
    $('#quantity_' + id).data('quantity', quantity + value);
}

function GrabarCombo() {

    var data = {
        ComboId: $('#Id').val(),
        Name: $('#Name').val(),
        Description: $('#Description').val(),
        Price: $('#Price').val(),
        Type: $('#Type').val(),
        Drinks: [],
        Sides: [],
        Products: []
    };

    $("input[id^='bebidas_']").each(function (item, index) {
        if ($(this).prop("checked")) {
            data.Drinks.push({ idCombo: data.ComboId, Type: "D", Quantity: $("#CantidadBebidas").val(), idDetail: $(this).val() });
        }
    });

    $("input[id^='guarniciones_']").each(function (item, index) {
        if ($(this).prop("checked")) {
            data.Sides.push({ idCombo: data.ComboId, Type: "S", Quantity: $("#CantidadGuarniciones").val(), idDetail: $(this).val() });
        }
    });

    $("input[id^='productos_']").each(function (item, index) {
        if ($(this).prop("checked")) {
            data.Products.push({ idCombo: data.ComboId, Type: "P", Quantity: $("#CantidadProductos").val(), idDetail: $(this).val() });
        }
    });

    console.log(data);

    $.ajax({
        url: '/api/Admin/GrabarCombo',
        type: 'POST',
        dataType: "JSON",
        contentType: "application/json; charset=utf-8",
        data: JSON.stringify(data)
    }).done(function (data) {
        if (data.Status == "OK") {
            
        }
    });
}