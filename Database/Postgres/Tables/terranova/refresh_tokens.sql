CREATE TABLE terranova.refresh_tokens (
  id char(26) PRIMARY KEY,
  user_id char(26) NOT NULL,
  token_hash varchar(255) NOT NULL,        -- hash del refresh token
  jti varchar(100),                        -- JWT ID (opcional)
  expires_at timestamptz NOT NULL,
  is_revoked boolean NOT NULL DEFAULT FALSE,
  revoked_at timestamptz,
  replaced_by_token_id char(26),
  created_at timestamptz NOT NULL DEFAULT now(),
  user_agent varchar(500),
  ip_address varchar(45)
);

-- Foreign Keys
ALTER TABLE terranova.refresh_tokens ADD CONSTRAINT fk_refresh_tokens_user FOREIGN KEY (user_id) REFERENCES terranova.users(id);
ALTER TABLE terranova.refresh_tokens ADD CONSTRAINT fk_refresh_tokens_replaced FOREIGN KEY (replaced_by_token_id) REFERENCES terranova.refresh_tokens(id);

-- Checks and Uniques
CREATE UNIQUE INDEX uq_refresh_tokens_jti ON terranova.refresh_tokens (jti) WHERE jti IS NOT NULL;
CREATE UNIQUE INDEX uq_refresh_tokens_user_not_revoked ON terranova.refresh_tokens (user_id) WHERE is_revoked = FALSE;