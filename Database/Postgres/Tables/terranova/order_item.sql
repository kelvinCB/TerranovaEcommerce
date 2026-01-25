CREATE TABLE terranova.order_item (
  id char(26) PRIMARY KEY,
  order_id char(26) NOT NULL,
  product_id char(26) NOT NULL,
  quantity char(26) NOT NULL,
  unit_price numeric(18, 2),
  created_at timestamptz,
  update_at timestamptz,
  is_deleted boolean NOT NULL
);

ALTER TABLE terranova.order_item ADD CONSTRAINT fk_order_item_order FOREIGN KEY (order_id) REFERENCES terranova.orders (id);
ALTER TABLE terranova.order_item ADD CONSTRAINT fk_order_item_product FOREIGN KEY (product_id) REFERENCES terranova.product (id);