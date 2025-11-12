# User Configuration Management - Best Practice YAML Structure

## YAML File Structure (per user)
Each user gets one YAML file: `{username}_configurations.yaml`

```yaml
BookingResourceConfig:
  data:
    bookingResourceName: "Conference Room A"
    timezone: "Europe/Stockholm"
    slotDuration: 30
    capacity: 2
    # ... rest of config
  lastUpdated: "2024-11-03T14:30:22Z"
  resourceName: "BookingResourceConfig"

UserPreferences:
  data:
    theme: "dark"
    language: "sv"
    notifications: true
  lastUpdated: "2024-11-03T14:30:22Z"
  resourceName: "UserPreferences"

AnotherConfig:
  data:
    # ... any other configuration
  lastUpdated: "2024-11-03T14:30:22Z"
  resourceName: "AnotherConfig"
```

## API Endpoints

### 1. SAVE Configuration - POST /api/settings
```json
{
  "userId": "123e4567-e89b-12d3-a456-426614174000",
  "userName": "johndoe",
  "configurationName": "BookingResourceConfig",
  "configurationData": "{\"bookingResourceName\":\"Conference Room A\",\"timezone\":\"Europe/Stockholm\"}"
}
```

### 2. GET All User Configurations - GET /api/settings/{userName}
Returns all configurations for the user

### 3. GET Specific Configuration - GET /api/settings/{userName}/{configName}
Returns specific configuration like "BookingResourceConfig"

### 4. UPDATE Configuration - PUT /api/settings/{userName}/{configName}
```json
{
  "configurationData": "{\"bookingResourceName\":\"Updated Room\",\"timezone\":\"Europe/Stockholm\"}"
}
```

### 5. DELETE Configuration - DELETE /api/settings/{userName}/{configName}

## Frontend Update (for your BookingResourceConfigPage)

Replace your current form submission with this:

```javascript
async function handleSubmit(e) {
  e.preventDefault();
  setSaving(true);
  setError('');
  
  const formData = {
    bookingResourceName: resourceName,
    offsetForResource,
    ...config,
  };
  
  if (!user) {
    setError('User not authenticated');
    return;
  }
  
  const requestBody = {
    userId: `00000000-0000-0000-0000-${user.id.toString().padStart(12, '0')}`,
    userName: user.userName,
    configurationName: resourceName || "BookingResourceConfig", // Use resource name as config name
    configurationData: JSON.stringify(formData)
  };

  try {
    const response = await fetch(`${import.meta.env.VITE_API_BASE_URL}/settings`, {
      method: 'POST',
      headers: { 'Content-Type': 'application/json' },
      body: JSON.stringify(requestBody)
    });

    if (!response.ok) throw new Error(`Failed to save: ${response.status}`);
    
    alert('Configuration saved successfully!');
    navigate('/dashboard/bookings');
  } catch (err) {
    setError(`Failed to save: ${err.message}`);
  } finally {
    setSaving(false);
  }
}
```

## Frontend Loading Configuration

Replace your fetchResource function with this:

```javascript
useEffect(() => {
  async function fetchResource() {
    if (!user || !resourceName) return;
    
    try {
      // Get specific configuration by name
      const response = await fetch(
        `${import.meta.env.VITE_API_BASE_URL}/settings/${user.userName}/${resourceName}`
      );
      
      if (response.ok) {
        const result = await response.json();
        const configData = result.configuration.data;
        
        // Set your state with the loaded configuration
        setResourceName(configData.bookingResourceName || '');
        setOffsetForResource(configData.offsetForResource || { startOffset: -100, endOffset: 364 });
        setConfig({
          timezone: configData.timezone || 'Europe/Stockholm',
          slotDuration: configData.slotDuration || 30,
          capacity: configData.capacity || 2,
          daysOfWeek: Array.isArray(configData.daysOfWeek) && configData.daysOfWeek.length === 7
            ? configData.daysOfWeek
            : defaultConfig.daysOfWeek,
        });
      } else {
        // Configuration not found, use defaults
        console.log('Configuration not found, using defaults');
      }
    } catch (err) {
      console.error('Failed to load configuration:', err);
      setError('Failed to load configuration');
    }
  }
  
  fetchResource();
}, [user, resourceName]);
```

## Benefits of This Structure

1. **One file per user** - `johndoe_configurations.yaml`
2. **Multiple named configurations** - Each config has a clear name
3. **Easy retrieval** - Get specific config by name
4. **Timestamps** - Track when each config was last updated
5. **Structured data** - Each config has metadata and data sections
6. **Simple frontend** - Clear API endpoints for CRUD operations

This approach gives you a clean, scalable way to manage multiple configurations per user while keeping the YAML structure organized and accessible!