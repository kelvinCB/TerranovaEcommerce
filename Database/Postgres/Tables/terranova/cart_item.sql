CREATE TABLE terranova.cart_item (
  id char(26) PRIMARY KEY,
  cart_id char(26) NOT NULL,
  product_id char(26) NOT NULL,
  quantity char(26) NOT NULL,
  create_at timestamptz
);

ALTER TABLE terranova.cart_item ADD CONSTRAINT fk_cart_item_cart FOREIGN KEY (cart_id) REFERENCES terranova.cart (id);
ALTER TABLE terranova.cart_item ADD CONSTRAINT fk_cart_item_product FOREIGN KEY (product_id) REFERENCES terranova.product (id);