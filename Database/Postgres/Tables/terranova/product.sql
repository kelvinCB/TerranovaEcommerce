CREATE TABLE terranova.product (
  id char(26) PRIMARY KEY,
  name varchar(100) NOT NULL,
  description text,
  price numeric(18, 2) NOT NULL,
  stock char(26) NOT NULL,
  category_id char(26) NOT NULL,
  sub_category_id char(26),
  sku varchar(50),
  created_at timestamptz,
  update_at timestamptz,
  is_deleted boolean NOT NULL
);

ALTER TABLE terranova.product ADD CONSTRAINT fk_product_category FOREIGN KEY (category_id) REFERENCES terranova.category (id);
ALTER TABLE terranova.product ADD CONSTRAINT fk_product_sub_category FOREIGN KEY (sub_category_id) REFERENCES terranova.sub_category (id);