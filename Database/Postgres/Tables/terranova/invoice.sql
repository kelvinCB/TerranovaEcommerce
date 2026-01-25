CREATE TABLE terranova.invoice (
  id char(26) PRIMARY KEY,
  invoice_number varchar(50) NOT NULL,
  order_id char(26) NOT NULL,
  invoice_status_id char(26) NOT NULL,
  total_amount numeric(18, 2) NOT NULL,
  shipping_amount numeric(18, 2) NOT NULL,
  invoice_date timestamptz NOT NULL,
  created_at timestamptz,
  update_at timestamptz,
  is_deleted boolean NOT NULL
);

ALTER TABLE terranova.invoice ADD CONSTRAINT fk_invoice_invoice_status FOREIGN KEY (invoice_status_id) REFERENCES terranova.invoice_status (id);
ALTER TABLE terranova.invoice ADD CONSTRAINT fk_invoice_order FOREIGN KEY (order_id) REFERENCES terranova.orders (id);