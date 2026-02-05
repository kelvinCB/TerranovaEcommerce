CREATE TABLE terranova.payment (
  id char(26) PRIMARY KEY,
  order_id char(26) NOT NULL,
  payment_method_id char(26) NOT NULL,
  payment_status_id char(26) NOT NULL,
  amount numeric(18, 2) NOT NULL,
  transaction_reference varchar(100),
  paid_at timestamptz,
  created_at timestamptz NOT NULL DEFAULT NOW(),
  updated_at timestamptz NOT NULL DEFAULT NOW(),
  is_deleted boolean NOT NULL DEFAULT FALSE
);

-- Foreign Key
ALTER TABLE terranova.payment ADD CONSTRAINT fk_payment_order FOREIGN KEY (order_id) REFERENCES terranova.orders (id);
ALTER TABLE terranova.payment ADD CONSTRAINT fk_payment_payment_method FOREIGN KEY (payment_method_id) REFERENCES terranova.payment_method (id);
ALTER TABLE terranova.payment ADD CONSTRAINT fk_payment_payment_status FOREIGN KEY (payment_status_id) REFERENCES terranova.payment_status (id);

-- Checks and Uniques
ALTER TABLE terranova.payment ADD CONSTRAINT chk_payment_amount_positive CHECK (amount >= 0);
CREATE UNIQUE INDEX CONCURRENTLY IF NOT EXISTS uq_payment_transaction_reference ON terranova.payment (transaction_reference) WHERE transaction_reference IS NOT NULL;