CREATE TABLE terranova.user_roles (
  user_id char(26) NOT NULL,
  role_id char(26) NOT NULL,
  assigned_at timestamptz NOT NULL DEFAULT now(),
  PRIMARY KEY (user_id, role_id),
  CONSTRAINT fk_user_roles_user FOREIGN KEY (user_id) REFERENCES terranova.users(id),
  CONSTRAINT fk_user_roles_role FOREIGN KEY (role_id) REFERENCES terranova.roles(id)
);