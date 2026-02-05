CREATE TABLE terranova.payment_webhook_log (
  id char(26) PRIMARY KEY,
  payment_id char(26) NOT NULL,
  provider varchar(50) NOT NULL,
  event_type varchar(100) NOT NULL,
  payload jsonb NOT NULL,
  received_at timestamptz NOT NULL,
  processed_at timestamptz,
  status varchar(50) NOT NULL,
  error_message text,
  created_at timestamptz NOT NULL DEFAULT NOW(),
  updated_at timestamptz NOT NULL DEFAULT NOW()
);

-- Foreign Keys
ALTER TABLE terranova.payment_webhook_log ADD CONSTRAINT fk_payment_webhook_log_payment FOREIGN KEY (payment_id) REFERENCES terranova.payment (id);