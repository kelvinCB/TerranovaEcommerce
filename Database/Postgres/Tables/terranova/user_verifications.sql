CREATE TABLE terranova.user_verifications (
  id char(26) PRIMARY KEY,
  user_id char(26) NOT NULL,
  purpose varchar(30) NOT NULL,            -- e.g. email_verify, reset_password, mfa
  code_hash varchar(255) NOT NULL,         -- guarda hash, NO el código plano
  expires_at timestamptz NOT NULL,
  consumed_at timestamptz,
  created_at timestamptz NOT NULL DEFAULT now(),
  CONSTRAINT fk_user_verifications_user FOREIGN KEY (user_id) REFERENCES terranova.users(id)
);