import { createContext, useContext, useState, useEffect } from 'react';

const AuthContext = createContext(null);

// Admin user for the AdminInterface
const ADMIN_USER = {
  id: 0,
  username: 'AdminPanel',
  email: 'admin@indiequest.local',
  displayName: 'Admin',
  isAdmin: true,
  role: 'ADMIN'
};

export function AuthProvider({ children }) {
  const [user] = useState(ADMIN_USER);

  return (
    <AuthContext.Provider value={{ user }}>
      {children}
    </AuthContext.Provider>
  );
}

export function useAuth() {
  const context = useContext(AuthContext);
  if (!context) {
    throw new Error('useAuth must be used within an AuthProvider');
  }
  return context;
}
