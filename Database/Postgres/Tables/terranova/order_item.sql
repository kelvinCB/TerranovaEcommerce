CREATE TABLE terranova.order_item (
  id char(26) PRIMARY KEY,
  order_id char(26) NOT NULL,
  product_id char(26) NOT NULL,
  quantity integer NOT NULL,
  unit_price numeric(18, 2) NOT NULL,
  created_at timestamptz NOT NULL DEFAULT NOW(),
  updated_at timestamptz NOT NULL DEFAULT NOW(),
  is_deleted boolean NOT NULL DEFAULT FALSE
);

-- Foreign Keys
ALTER TABLE terranova.order_item ADD CONSTRAINT fk_order_item_order FOREIGN KEY (order_id) REFERENCES terranova.orders (id);
ALTER TABLE terranova.order_item ADD CONSTRAINT fk_order_item_product FOREIGN KEY (product_id) REFERENCES terranova.product (id);

-- Checks and Uniques
ALTER TABLE terranova.order_item ADD CONSTRAINT uq_order_item_order_product UNIQUE (order_id, product_id);
ALTER TABLE terranova.order_item ADD CONSTRAINT chk_order_item_quantity_positive CHECK (quantity > 0);
ALTER TABLE terranova.order_item ADD CONSTRAINT chk_order_item_unit_price_positive CHECK (unit_price >= 0);