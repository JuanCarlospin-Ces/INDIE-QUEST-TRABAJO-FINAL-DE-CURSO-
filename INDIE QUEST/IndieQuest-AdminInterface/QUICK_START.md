# Quick Start Guide - Admin Interface

## 🚀 Getting Started in 30 Seconds

### Prerequisites
- Node.js 16+ installed
- npm or yarn package manager
- Access to IndieQuest API running

### Step 1: Install Dependencies
```bash
cd IndieQuest-AdminInterface
npm install
```

### Step 2: Configure API Endpoint (if needed)
Check `src/api/client.js` for API base URL:
```javascript
const API_BASE_URL = process.env.VITE_API_URL || 'http://localhost:5000/api';
```

If needed, create `.env.local`:
```env
VITE_API_URL=http://your-api-server:port/api
```

### Step 3: Start Development Server
```bash
npm run dev
```

### Step 4: Access Admin Panel
Open your browser and navigate to:
```
http://localhost:5173
```

**That's it!** You should see:
- ✅ "ADMIN MODE" badge in sidebar
- ✅ "AdminPanel" user logged in
- ✅ Full administrative access
- ✅ No login screen

## 🎯 What You Can Do

### User Management
- 📋 View all users
- ✏️ Edit any user's information
- 🗑️ Delete any user
- 🔍 Filter by availability

### Post Management
- 📰 View all posts from all users
- ✏️ Edit any post
- 🗑️ Delete any post
- 📍 Create new posts

### Content Review
- 🔎 Search all content
- 📊 Browse community activity
- 🛠️ Manage all platform content

## 📱 Available Commands

```bash
# Development server
npm run dev

# Build for production
npm run build

# Preview production build
npm run preview

# Run tests (if configured)
npm run test

# Lint code
npm run lint
```

## 🔧 Troubleshooting

### "API Connection Failed"
- ✅ Ensure IndieQuest API is running
- ✅ Check API URL in `.env.local`
- ✅ Verify CORS settings on API

### "Pages Not Loading"
- ✅ Clear browser cache
- ✅ Check browser console for errors
- ✅ Verify Node.js version (16+)

### "Buttons Not Working"
- ✅ Open browser console (F12)
- ✅ Check for JavaScript errors
- ✅ Verify API endpoints are accessible

### "Styling Looks Wrong"
- ✅ Run `npm install` again
- ✅ Clear node_modules and reinstall
- ✅ Restart development server

## 📂 Project Structure

```
IndieQuest-AdminInterface/
├── src/
│   ├── api/              # API client calls
│   ├── components/       # React components
│   ├── context/          # Auth context (auto-admin)
│   ├── pages/            # Page components
│   ├── styles/           # CSS styles
│   ├── utils/            # Utility functions
│   ├── App.jsx           # Main app component
│   └── main.jsx          # Entry point
├── public/               # Static assets
├── package.json          # Dependencies
├── vite.config.js        # Vite config
├── ADMIN_PANEL_README.md # Full documentation
├── ADMIN_TESTING_GUIDE.md # Testing guide
├── CHANGELOG.md          # Change log
└── QUICK_START.md        # This file
```

## 🔑 Admin Credentials

The admin account is automatically configured:

| Field | Value |
|-------|-------|
| **Username** | AdminPanel |
| **Email** | admin@indiequest.local |
| **Role** | ADMIN |
| **ID** | 0 |
| **Status** | Auto-logged in |

**No password needed** - Direct access!

## 🌐 Environment Variables

### Development (.env.local)
```env
# API Configuration
VITE_API_URL=http://localhost:5000/api

# Optional: Development settings
VITE_DEBUG=true
```

### Production (.env.production)
```env
# API Configuration
VITE_API_URL=https://api.indiequest.com/api

# Optional: Production settings
VITE_DEBUG=false
```

## 📡 API Integration

The admin panel communicates with these API endpoints:

### Users
- `GET /users` - List all users (paged)
- `GET /users/{id}` - Get user details
- `PUT /users/{id}` - Update user
- `DELETE /users/{id}` - Delete user

### Posts
- `GET /posts` - List all posts (paged)
- `GET /posts/{id}` - Get post details
- `POST /posts` - Create new post
- `PUT /posts/{id}` - Update post
- `DELETE /posts/{id}` - Delete post

## 🆘 Need Help?

1. **Documentation**: Read `ADMIN_PANEL_README.md`
2. **Testing**: Follow `ADMIN_TESTING_GUIDE.md`
3. **Changes**: Review `CHANGELOG.md`
4. **Issues**: Check browser console for error messages

## 🚨 Important Notes

⚠️ **SECURITY WARNING**:
- This panel provides **unrestricted access**
- **No authentication required**
- **Full destructive capabilities**
- **Should NOT be exposed to public**
- Deploy behind firewall or IP restrictions

## 📊 Development Tips

### Hot Module Replacement
The development server supports HMR - changes are reflected instantly in the browser.

### Browser DevTools
- Press `F12` to open developer tools
- Check Console tab for errors
- Use Network tab to monitor API calls

### Debugging
1. Add `debugger;` statement in code
2. Open DevTools (F12)
3. Refresh page to break at debugger
4. Inspect variables and state

## 🎓 Learning More

- **React**: https://react.dev
- **React Router**: https://reactrouter.com
- **Vite**: https://vitejs.dev
- **IndieQuest API Docs**: See `../API_UML_DIAGRAM.md`

## 🎉 You're Ready!

The admin panel is now:
✅ Configured
✅ Ready to use
✅ No login required
✅ Full admin access enabled

Start managing your IndieQuest community!

---

**Need to stop the server?**
Press `Ctrl+C` in the terminal where the dev server is running.

**Happy administrating! 🚀**
