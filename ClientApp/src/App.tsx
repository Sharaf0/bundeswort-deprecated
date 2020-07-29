import React from 'react';
import { Route } from 'react-router';
import { Layout } from './components/Layout';
import { AddVideo } from './components/AddVideo';
import { Search } from './components/Search';

export default function App() {
  return (
    <Layout>
      <Route exact path='/add-video' component={AddVideo} />
      <Route exact path='/' component={Search} />
    </Layout>
  );
}
