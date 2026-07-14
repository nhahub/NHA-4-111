
const AUTH_TOKEN_KEY = "fitcore_token";
const AUTH_USER_KEY = "fitcore_user";

function saveSession(authResponse) {
    localStorage.setItem(AUTH_TOKEN_KEY, authResponse.token);
    localStorage.setItem(AUTH_USER_KEY, JSON.stringify({
        userId: authResponse.userID ?? authResponse.userId,
        fullName: authResponse.fullName,
        email: authResponse.email,
        roles: authResponse.roles || [],
    }));
}

function getToken() {
    return localStorage.getItem(AUTH_TOKEN_KEY);
}

function getCurrentUser() {
    const raw = localStorage.getItem(AUTH_USER_KEY);
    return raw ? JSON.parse(raw) : null;
}

function logout() {
    localStorage.removeItem(AUTH_TOKEN_KEY);
    localStorage.removeItem(AUTH_USER_KEY);
    window.location.href = "/html/Auth/login.html";
}


function decodeTokenPayload(token) {
    try {
        const payload = token.split(".")[1];
        const json = decodeURIComponent(atob(payload.replace(/-/g, "+").replace(/_/g, "/"))
            .split("").map(c => "%" + c.charCodeAt(0).toString(16).padStart(2, "0")).join(""));
        return JSON.parse(json);
    } catch (e) {
        return null;
    }
}


function requireRole(allowedRoles) {
    const token = getToken();

    if (!token) {
        window.location.href = "/html/Auth/login.html";
        return null;
    }

    const payload = decodeTokenPayload(token);


    const roleClaim = payload && (
        payload["role"] ||
        payload["http://schemas.microsoft.com/ws/2008/06/identity/claims/role"]
    );

    const roles = Array.isArray(roleClaim) ? roleClaim : (roleClaim ? [roleClaim] : []);
    console.log("Found Roles:", roles);

    const isAllowed = allowedRoles.some(r => roles.includes(r));
    if (!isAllowed) {
        window.location.href = "/html/access-denied.html";
        return null;
    }

    return roles;
}

async function authFetch(url, options = {}) {
    const token = getToken();
    const headers = Object.assign(
        { "Content-Type": "application/json" },
        options.headers || {},
        token ? { Authorization: `Bearer ${token}` } : {}
    );

    const response = await fetch(url, Object.assign({}, options, { headers }));

    if (response.status === 401) {
        logout();
        throw new Error("Session expired. Please log in again.");
    }

    let data = null;
    try { data = await response.json(); } catch (e) { }

    if (!response.ok) {
        const message = (data && (data.message || data.Message)) || `Request failed (${response.status})`;
        const err = new Error(message);
        err.errors = data && (data.errors || data.Errors);
        throw err;
    }

    return data;
}