CREATE TABLE terranova.sub_category (
  id char(26) PRIMARY KEY,
  name varchar(100) NOT NULL,
  description text,
  category_id char(26) NOT NULL,
  created_at timestamptz NOT NULL DEFAULT NOW(),
  updated_at timestamptz NOT NULL DEFAULT NOW(),
  is_deleted boolean NOT NULL DEFAULT FALSE
);

-- Foreign Keys
ALTER TABLE terranova.sub_category ADD CONSTRAINT fk_sub_category_category FOREIGN KEY (category_id) REFERENCES terranova.category (id);

-- Checks and Uniques
ALTER TABLE terranova.sub_category ADD CONSTRAINT uq_sub_category_name_per_category UNIQUE (name, category_id);