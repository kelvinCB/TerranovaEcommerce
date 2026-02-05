CREATE TABLE terranova.product_sub_category (
  id char(26),
  product_id char(26) NOT NULL,
  sub_category_id char(26) NOT NULL,
  created_at timestamptz NOT NULL DEFAULT NOW(),
  PRIMARY KEY(product_id, sub_category_id)
);

-- Foreign keys
ALTER TABLE terranova.product_sub_category ADD CONSTRAINT fk_product_sub_category_product FOREIGN KEY (product_id) REFERENCES terranova.product (id);
ALTER TABLE terranova.product_sub_category ADD CONSTRAINT fk_product_sub_category_sub_category FOREIGN KEY (sub_category_id) REFERENCES terranova.sub_category (id);