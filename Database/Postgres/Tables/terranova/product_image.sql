CREATE TABLE terranova.product_image (
  id char(26) PRIMARY KEY,
  product_id char(26) NOT NULL,
  image_url varchar(500) NOT NULL,
  is_main boolean NOT NULL DEFAULT FALSE,
  created_at timestamptz NOT NULL DEFAULT NOW(),
  updated_at timestamptz NOT NULL DEFAULT NOW(),
  is_deleted boolean NOT NULL DEFAULT FALSE
);

-- Foreign Keys
ALTER TABLE terranova.product_image ADD CONSTRAINT fk_product_image_product FOREIGN KEY (product_id) REFERENCES terranova.product (id);

-- Checks and Uniques

CREATE UNIQUE INDEX only_one_main_product_image ON terranova.product_image(product_id) WHERE is_main = TRUE;