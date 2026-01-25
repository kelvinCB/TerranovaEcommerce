CREATE TABLE terranova.product_sub_category (
  id char(26) PRIMARY KEY,
  product_id char(26) NOT NULL,
  sub_category_id char(26) NOT NULL,
  created_at timestamptz,
  update_at timestamptz,
  is_deleted boolean NOT NULL
);

ALTER TABLE terranova.product_sub_category ADD CONSTRAINT fk_product_sub_category_product FOREIGN KEY (product_id) REFERENCES terranova.product (id);
ALTER TABLE terranova.product_sub_category ADD CONSTRAINT fk_product_sub_category_sub_category FOREIGN KEY (sub_category_id) REFERENCES terranova.sub_category (id);