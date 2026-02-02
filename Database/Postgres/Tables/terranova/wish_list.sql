CREATE TABLE terranova.wish_list (
  id char(26) PRIMARY KEY,
  user_id char(26) NOT NULL UNIQUE,
  created_at timestamptz NOT NULL DEFAULT NOW(),
  updated_at timestamptz,
  is_deleted boolean NOT NULL DEFAULT FALSE
);

ALTER TABLE terranova.wish_list ADD CONSTRAINT fk_wish_list_user FOREIGN KEY (user_id) REFERENCES terranova.users (id);