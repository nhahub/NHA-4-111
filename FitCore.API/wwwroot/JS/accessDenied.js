
const user = getCurrentUser();
const redict= () => {
    redirectForRoles(user.roles[0]);
}

