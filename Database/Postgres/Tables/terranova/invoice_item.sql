CREATE TABLE terranova.invoice_item (
  id char(26) PRIMARY KEY,
  invoice_id char(26) NOT NULL,
  product_id char(26) NOT NULL,
  quantity integer NOT NULL,
  unit_price numeric(18, 2) NOT NULL,
  created_at timestamptz NOT NULL DEFAULT NOW(),
  updated_at timestamptz NOT NULL DEFAULT NOW(),
  is_deleted boolean NOT NULL DEFAULT FALSE
);

-- Foreign Keys
ALTER TABLE terranova.invoice_item ADD CONSTRAINT fk_invoice_item_invoice FOREIGN KEY (invoice_id) REFERENCES terranova.invoice (id);
ALTER TABLE terranova.invoice_item ADD CONSTRAINT fk_invoice_item_product FOREIGN KEY (product_id) REFERENCES terranova.product (id);

-- Checks and Uniques
ALTER TABLE terranova.invoice_item ADD CONSTRAINT uq_invoice_item_invoice_product UNIQUE (invoice_id, product_id);
ALTER TABLE terranova.invoice_item ADD CONSTRAINT chk_invoice_item_quantity_positive CHECK (quantity > 0);
ALTER TABLE terranova.invoice_item ADD CONSTRAINT chk_invoice_item_unit_price_positive CHECK (unit_price >= 0);