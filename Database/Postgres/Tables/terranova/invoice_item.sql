CREATE TABLE terranova.invoice_item (
  id char(26) PRIMARY KEY,
  invoice_id char(26) NOT NULL,
  product_id char(26) NOT NULL,
  quantity char(26) NOT NULL,
  unit_price numeric(18, 2),
  created_at timestamptz,
  update_at timestamptz,
  is_deleted boolean NOT NULL
);

ALTER TABLE terranova.invoice_item ADD CONSTRAINT fk_invoice_item_invoice FOREIGN KEY (invoice_id) REFERENCES terranova.invoice (id);
ALTER TABLE terranova.invoice_item ADD CONSTRAINT fk_invoice_item_product FOREIGN KEY (product_id) REFERENCES terranova.product (id);