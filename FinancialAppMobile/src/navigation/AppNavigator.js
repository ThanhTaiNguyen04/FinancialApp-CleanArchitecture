import React from 'react';
import { NavigationContainer } from '@react-navigation/native';
import { createStackNavigator } from '@react-navigation/stack';

// Import screens
import SplashScreen from '../screens/auth/SplashScreen';
import WelcomeScreen from '../screens/auth/WelcomeScreen';
import LoginScreen from '../screens/auth/LoginScreen';
import RegisterScreen from '../screens/auth/RegisterScreen';
import MainScreen from '../screens/main/MainScreen';

import { SCREEN_NAMES } from '../utils/constants';

const Stack = createStackNavigator();

const AppNavigator = () => {
  return (
    <NavigationContainer>
      <Stack.Navigator 
        initialRouteName={SCREEN_NAMES.SPLASH}
        screenOptions={{
          headerShown: false,
        }}
      >
        <Stack.Screen 
          name={SCREEN_NAMES.SPLASH} 
          component={SplashScreen} 
        />
        <Stack.Screen 
          name={SCREEN_NAMES.WELCOME} 
          component={WelcomeScreen}
        />
        <Stack.Screen 
          name={SCREEN_NAMES.LOGIN} 
          component={LoginScreen}
        />
        <Stack.Screen 
          name={SCREEN_NAMES.REGISTER} 
          component={RegisterScreen}
        />
        <Stack.Screen 
          name={SCREEN_NAMES.MAIN} 
          component={MainScreen}
        />
      </Stack.Navigator>
    </NavigationContainer>
  );
};

export default AppNavigator;