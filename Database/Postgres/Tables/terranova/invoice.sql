CREATE TABLE terranova.invoice (
  id char(26) PRIMARY KEY,
  invoice_number varchar(50) NOT NULL UNIQUE,
  order_id char(26) NOT NULL UNIQUE,
  invoice_status_id char(26) NOT NULL,
  total_amount numeric(18, 2) NOT NULL,
  shipping_amount numeric(18, 2) NOT NULL,
  invoice_date timestamptz NOT NULL,
  created_at timestamptz NOT NULL DEFAULT NOW(),
  updated_at timestamptz NOT NULL DEFAULT NOW(),
  is_deleted boolean NOT NULL DEFAULT FALSE
);

-- Foreign Keys
ALTER TABLE terranova.invoice ADD CONSTRAINT fk_invoice_invoice_status FOREIGN KEY (invoice_status_id) REFERENCES terranova.invoice_status (id);
ALTER TABLE terranova.invoice ADD CONSTRAINT fk_invoice_order FOREIGN KEY (order_id) REFERENCES terranova.orders (id);

-- Checks and Uniques
ALTER TABLE terranova.invoice ADD CONSTRAINT chk_invoice_total_amount_positive CHECK (total_amount >= 0);
ALTER TABLE terranova.invoice ADD CONSTRAINT chk_invoice_shipping_amount_positive CHECK (shipping_amount >= 0);