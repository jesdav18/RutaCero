# Modelo de dominio

Money une monto decimal y moneda y rechaza operaciones entre monedas distintas. FinancialAccount conserva una referencia parcial. Debt solo reduce principal cuando DebtPayment contiene una asignación confirmada. PaymentObligation deriva su estado desde la fecha máxima y pagos aplicados.

AvailableCash protege obligaciones, gastos esenciales, reserva, movimientos pendientes y buffers. CapitalPaymentRecommendation selecciona una deuda mediante Avalanche, Snowball, CashFlowRelease o Hybrid y se bloquea cuando los datos o la liquidez no son seguros.
