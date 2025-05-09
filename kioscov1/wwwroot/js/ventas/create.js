$(document).ready(function () {


    $(document).ready(function () {
        $('#productoSelect').select2({
            placeholder: "Buscar producto...",
            allowClear: true
        });
    });

    $("#btnAgregar").on("click", function () {


        var total = parseFloat($("#productoSelect").find(":selected").data("precio")) * parseFloat($("#txtcantidad").val())


        $("#tbProducto tbody").append(

            $("<tr>").append(


                $("<td>").text($("#productoSelect").val()),
                $("<td>").text($("#productoSelect").find(":selected").data("nombre")),
                $("<td>").addClass("cantidad").text($("#txtcantidad").val()),
                $("<td>").text($("#productoSelect").find(":selected").data("precio")),
                $("<td>").addClass("total").text(total),
                $("<td>").append(
                    $('<button class="btn btn-sm btn-warning btnEditar me-1"> Editar </button>'),
                    $('<button class="btn btn-sm btn-danger btnEliminar">Eliminar </button>')
                )
            )

        )
        console.log("Salud!")
        $("#txtproducto").val("")
        $("#txtprecio").val("")
        $("#txtcantidad").val("1")
        $("#txtproducto").focus();
    })

    $("#codigoBarras").on("keypress", function (e) {
        if (e.which == 13) {
            var codigo = parseInt($(this).val().trim());
            const producto = productos.find(p => p.codigoBarra === codigo);

            if (producto) {
                var cantidad = parseInt($("#txtcantidad").val());
                const total = cantidad * producto.precio;

                $("#tbProducto tbody").append(
                    $("<tr>").append(
                        $("<td>").text(producto.id),
                        $("<td>").text(producto.nombre),
                        $("<td>").addClass("cantidad").text(cantidad),
                        $("<td>").text(producto.precio),
                        $("<td>").addClass("total").text(total),
                        $("<td>").append(
                            $('<button class="btn btn-sm btn-warning btnEditar me-1"> Editar </button>'),
                            $('<button class="btn btn-sm btn-danger btnEliminar">Eliminar </button>')
                        )
                    )
                );

                $("#txtcantidad").val("1");
            } else {
                alert("Producto no encontrado");
            }
        }

    })

    $("#btnTerminar").on("click", function () {
        var oDetalle = [];
        var total = 0;

        $("#tbProducto > tbody > tr").each(function (i, tr) {
            oDetalle.push({
                productoId: parseInt($(tr).find('td:eq(0)').text()),
                cantidad: parseInt($(tr).find('td:eq(2)').text()),
                precioUnitario: parseFloat($(tr).find('td:eq(3)').text()),
                precioTotal: parseFloat($(tr).find('td:eq(4)').text())

            })
            total = total + parseFloat($(tr).find('td:eq(4)').text());
        })

        var fechan = new Date();
        fechan = new Date(fechan.getTime() - (fechan.getTimezoneOffset() * 60000));
        var fechanLocal = fechan.toISOString();

        var oVentaVM = {
            Venta: {
                fecha: fechanLocal,
                usuarioId: usuarioId,
                importe: total
            },
            Detalles: oDetalle.map(d => ({
                ProductoId: d.productoId,
                Cantidad: d.cantidad,
                PrecioUnitario: d.precioUnitario,
                PrecioTotal: d.precioTotal
            }))
        };

        console.log(oVentaVM);

        jQuery.ajax({
            url: '@Url.Action("Create","Ventas1")',
            type: "POST",
            data: JSON.stringify(oVentaVM),
            datatype: "json",
            contentType: "application/json; charset=utf-8",
            success: function (data) {
                alert(data.message);
                location.reload();
            },
            error: function (xhr, status, error) {
                alert("Error al registrar la venta: " + xhr.responseText);
            }
        });
    });

    $("#tbProducto").on("click", ".btnEliminar", function () {
        $(this).closest("tr").remove();
    })

    $("#tbProducto").on("click", ".btnEditar", function () {
        const $row = $(this).closest("tr");
        const cantidadActual = parseInt($row.find(".cantidad").text());
        const nuevaCantidad = prompt("Editar cantidad:", cantidadActual);

        if (nuevaCantidad !== null && !isNaN(nuevaCantidad) && parseInt(nuevaCantidad) > 0) {
            const precio = parseFloat($row.find("td:eq(3)").text());
            const total = parseInt(nuevaCantidad) * precio;

            $row.find(".cantidad").text(nuevaCantidad);
            $row.find(".total").text(total.toFixed(2));
        }
    });

    function actualizarTotalGeneral() {
        var total = 0;
        $("#tbProducto > tbody > tr").each(function () {
            const precioTotal = parseFloat($(this).find(".total").text());
            if (!isNaN(precioTotal)) {
                total += precioTotal;
            }
        });

        $("#totalGeneral").text(total.toFixed(2));
    }
})