import { UserManager, WebStorageStateStore, type UserManagerSettings } from 'oidc-client-ts';
import { getApiBaseUrl } from '../api/api-client';

const trimTrailingSlash = (value: string) => value.replace(/\/+$/, '');

const getAuthority = () => {
  const baseUrl = getApiBaseUrl();
  if (baseUrl) {
    return trimTrailingSlash(baseUrl);
  }

  return trimTrailingSlash(window.location.origin);
};

const buildSettings = (): UserManagerSettings => {
  const authority = getAuthority();
  const origin = window.location.origin;

  return {
    authority,
    client_id: 'hmmh-ui',
    scope: 'openid api offline_access',
    response_type: 'code',
    redirect_uri: `${origin}/oidc/callback`,
    post_logout_redirect_uri: `${origin}/oidc/logout`,
    userStore: new WebStorageStateStore({ store: window.sessionStorage }),
    automaticSilentRenew: true,
    loadUserInfo: false,
    filterProtocolClaims: true,
    metadata: {
      issuer: authority,
      token_endpoint: `${authority}/connect/token`,
    },
  };
};

let userManager: UserManager | null = null;

export const getUserManager = () => {
  if (!userManager) {
    userManager = new UserManager(buildSettings());
  }

  return userManager;
};
