CREATE TABLE terranova.orders (
  id char(26) PRIMARY KEY,
  user_id char(26) NOT NULL,
  order_status_id char(26) NOT NULL,
  total_amount numeric(18, 2) NOT NULL,
  shipping_amount numeric(18, 2) NOT NULL,
  shipping_address_id char(26) NOT NULL,
  created_at timestamptz,
  update_at timestamptz,
  is_deleted boolean NOT NULL
);

ALTER TABLE terranova.orders ADD CONSTRAINT fk_order_order_status FOREIGN KEY (order_status_id) REFERENCES terranova.order_status (id);
ALTER TABLE terranova.orders ADD CONSTRAINT fk_order_shipping_address FOREIGN KEY (shipping_address_id) REFERENCES terranova.shipping_address (id);
ALTER TABLE terranova.orders ADD CONSTRAINT fk_order_user FOREIGN KEY (user_id) REFERENCES terranova.users (id);