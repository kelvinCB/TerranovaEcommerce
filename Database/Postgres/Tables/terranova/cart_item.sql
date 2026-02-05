CREATE TABLE terranova.cart_item (
  id char(26) PRIMARY KEY,
  cart_id char(26) NOT NULL,
  product_id char(26) NOT NULL,
  quantity integer NOT NULL,
  is_selected boolean NOT NULL DEFAULT FALSE,
  created_at timestamptz NOT NULL DEFAULT NOW(),
  updated_at timestamptz NOT NULL DEFAULT NOW()
);

-- Foreign Keys
ALTER TABLE terranova.cart_item ADD CONSTRAINT fk_cart_item_cart FOREIGN KEY (cart_id) REFERENCES terranova.cart (id);
ALTER TABLE terranova.cart_item ADD CONSTRAINT fk_cart_item_product FOREIGN KEY (product_id) REFERENCES terranova.product (id);

-- Checks and Uniques
ALTER TABLE terranova.cart_item ADD CONSTRAINT chk_cart_item_quantity_positive CHECK (quantity > 0);
ALTER TABLE terranova.cart_item ADD CONSTRAINT uq_cart_item_cart_product UNIQUE (cart_id, product_id);